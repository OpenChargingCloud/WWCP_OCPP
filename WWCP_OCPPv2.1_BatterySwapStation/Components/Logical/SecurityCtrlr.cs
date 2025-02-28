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
    /// Logical Component responsible for configuration relating to security of communications between battery swap station and CSMS
    /// (HTTP Basic Authentication settings).
    /// </summary>
    public class SecurityCtrlr : ALogicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// The battery swap station identity.
        /// </summary>
        public String?           Identity                          { get; set; }

        /// <summary>
        /// The HTTP Basic Authentication password.
        /// </summary>
        public String?           BasicAuthPassword                 { get; set; }

        /// <summary>
        /// This configuration variable is used to set the organization name of the CSO or an organization trusted by the CSO.
        /// This organization name is used to specify the subject field in the client certificate.
        /// </summary>
        [Mandatory]
        public String            OrganizationName                  { get; set; }

        /// <summary>
        /// Amount of certificates currently installed on the battery swap station.
        /// </summary>
        [Mandatory]
        public UInt16            CertificateEntries                { get; set; }

        /// <summary>
        /// This configuration variable is used to report the security profile used by the battery swap station.
        /// </summary>
        [Mandatory]
        public SecurityProfiles  SecurityProfile                   { get; set; }

        /// <summary>
        /// When set to true, only one certificate (plus a temporarily fallback certificate) of certificateType CSMSRootCertificate is allowed to be installed at a time.
        /// When installing a new CSMS Root certificate, the new certificate SHALL replace the old one AND the new CSMS Root Certificate MUST be signed by the old CSMS Root Certificate it is replacing.
        /// </summary>
        public Boolean?          AdditionalRootCertificateCheck    { get; set; }

        /// <summary>
        /// This configuration variable can be used to limit the size of the 'certificateChain' field from the CertificateSignedRequest PDU.
        /// This value SHOULD NOT be set too small. The smaller this value, the less security architectures the Charging Station will support.
        /// It is RECOMMENDED to set at least a size of 5600. This will allow the Charging Station to support most security architectures.
        /// </summary>
        public UInt32?           MaxCertificateChainSize           { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new security controller.
        /// </summary>
        /// <param name="OrganizationName">This configuration variable is used to set the organization name of the CSO or an organization trusted by the CSO. This organization name is used to specify the subject field in the client certificate.</param>
        /// <param name="CertificateEntries">Amount of certificates currently installed on the battery swap station.</param>
        /// <param name="SecurityProfile">This configuration variable is used to report the security profile used by the battery swap station.</param>
        /// 
        /// <param name="Identity">The battery swap station identity.</param>
        /// <param name="BasicAuthPassword">The HTTP Basic Authentication password</param>
        /// <param name="AdditionalRootCertificateCheck">When set to true, only one certificate (plus a temporarily fallback certificate) of certificateType CSMSRootCertificate is allowed to be installed at a time. When installing a new CSMS Root certificate, the new certificate SHALL replace the old one AND the new CSMS Root Certificate MUST be signed by the old CSMS Root Certificate it is replacing.</param>
        /// <param name="MaxCertificateChainSize">This configuration variable can be used to limit the size of the 'certificateChain' field from the CertificateSignedRequest PDU.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public SecurityCtrlr(String            OrganizationName,
                             UInt16            CertificateEntries,
                             SecurityProfiles  SecurityProfile,

                             String?           Identity                         = null,
                             String?           BasicAuthPassword                = null,
                             Boolean?          AdditionalRootCertificateCheck   = null,
                             UInt32?           MaxCertificateChainSize          = null,

                             String?           Instance                         = null,
                             CustomData?       CustomData                       = null)

            : base(nameof(SecurityCtrlr),
                   Instance,
                   I18NString.Create(
                       "Logical Component responsible for configuration relating to security of " +
                       "communications between battery swap station and CSMS (HTTP Basic Authentication settings)."
                   ),
                   CustomData)

        {

            this.OrganizationName                = OrganizationName;
            this.CertificateEntries              = CertificateEntries;
            this.SecurityProfile                 = SecurityProfile;

            this.Identity                        = Identity;
            this.BasicAuthPassword               = BasicAuthPassword;
            this.AdditionalRootCertificateCheck  = AdditionalRootCertificateCheck;
            this.MaxCertificateChainSize         = MaxCertificateChainSize;


            #region Identity

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Identity",
                    ValueGetter:      () => this.Identity,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String,
                                          MaxLimit:    48
                                      ),

                    Description:      I18NString.Create("The battery swap station identity (identifierString).")

                )
            );

            #endregion

            #region BasicAuthPassword

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "BasicAuthPassword",
                    ValueGetter:      () => this.BasicAuthPassword,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.WriteOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String,
                                          MaxLimit:    40
                                      ),

                    Description:      I18NString.Create(
                                          "The HTTP Basic Authentication password (16-40 alpha-numeric and special characters allowed " +
                                          "bypasswordString). This configuration variable is write-only, sothat it cannot be accidentally " +
                                          "stored in plaintext by the CSMS when it reads out allconfiguration variables. This configuration " +
                                          "variable is required unless only \"security profile 3 - TLS withclient side certificates\" is implemented."
                                      )

                )
            );

            #endregion

            #region OrganizationName

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "OrganizationName",
                    ValueGetter:      ()                   => this.OrganizationName,
                    ValueSetter:      (newValue, oldValue) => {

                                                                  if (newValue is null)
                                                                      return new ValueSetterResponse(this.OrganizationName, "The new OrganizationName must not be null!");

                                                                  if (oldValue is not null && oldValue != this.OrganizationName)
                                                                      return new ValueSetterResponse(this.OrganizationName, "The old OrganizationName is no longer valid!");

                                                                  this.OrganizationName = newValue;
                                                                  return new ValueSetterResponse(this.OrganizationName);

                                                              },

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create(
                                          "This configuration variable is used to set the organization name of the CSO or an organization trusted by the CSO. " +
                                          "This organization name is used to specify the subject field in the client certificate."
                                      )

                )
            );

            #endregion

            #region CertificateEntries

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "CertificateEntries",
                    ValueGetter:      () => this.CertificateEntries.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("Amount of certificates currently installed on the battery swap station.")

                )
            );

            #endregion

            #region SecurityProfile

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "SecurityProfile",
                    ValueGetter:      () => this.SecurityProfile.AsNumber().ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("This configuration variable is used to report the security profile used by the battery swap station.")

                )
            );

            #endregion

            #region AdditionalRootCertificateCheck

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "AdditionalRootCertificateCheck",
                    ValueGetter:      () => this.AdditionalRootCertificateCheck.HasValue
                                                ? this.AdditionalRootCertificateCheck.Value
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
                                          "When set to true, only one certificate (plus a temporarily fallback certificate) of certificateType " +
                                          "CSMSRootCertificate is allowed to be installed at a time. When installing a new CSMS Root certificate, " +
                                          "the new certificate SHALL replace the old one AND the new CSMS Root Certificate MUST be signed by the " +
                                          "old CSMS Root Certificate it is replacing."
                                      )

                )
            );

            #endregion

            #region MaxCertificateChainSize

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "MaxCertificateChainSize",
                    ValueGetter:      () => this.MaxCertificateChainSize?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadOnly
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer,
                                          MaxLimit:    10000
                                      ),

                    Description:      I18NString.Create(
                                          "When set to true, only one certificate (plus a temporarily fallback certificate) of certificateType " +
                                          "CSMSRootCertificate is allowed to be installed at a time. When installing a new CSMS Root certificate, " +
                                          "the new certificate SHALL replace the old one AND the new CSMS Root Certificate MUST be signed by the " +
                                          "old CSMS Root Certificate it is replacing."
                                      )

                )
            );

            #endregion


        }

        #endregion


    }

}
