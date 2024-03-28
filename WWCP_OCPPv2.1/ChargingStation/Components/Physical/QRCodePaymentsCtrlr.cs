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
    /// QR-Code settings for ad hoc payments.
    /// </summary>
    public class QRCodePaymentsCtrlr : APhysicalComponentConfig
    {

        #region Properties

        /// <summary>
        /// QR-Code payments controller enabled.
        /// </summary>
        public Boolean?             Enabled                             { get; set; }


        public String?              URLTemplate                         { get; set; }

        public TimeSpan?            ValidityTime                        { get; set; }

        public String?              HashAlgorithm                       { get; set; }

        public String?              SharedSecret                        { get; set; }

        public Byte?                Length                              { get; set; }

        public String?              Encoding                            { get; set; }

        public String?              QRCodeQuality                       { get; set; }

        public String?              Signature                           { get; set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new QR-Code payments controller.
        /// </summary>
        /// <param name="EVSE">An optional EVSE when component is located at EVSE level, also specifies the connector when component is located at connector level.</param>
        /// <param name="Enabled">QR-Code payments controller enabled.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public QRCodePaymentsCtrlr(EVSE?        EVSE            = null,
                                   Boolean?     Enabled         = null,
                                   String?      URLTemplate     = null,
                                   TimeSpan?    ValidityTime    = null,
                                   String?      HashAlgorithm   = null,
                                   String?      SharedSecret    = null,
                                   Byte?        Length          = null,
                                   String?      Encoding        = null,
                                   String?      QRCodeQuality   = null,
                                   String?      Signature       = null,

                                   String?      Instance        = null,
                                   CustomData?  CustomData      = null)

            : base(nameof(QRCodePaymentsCtrlr),
                   Instance,
                   EVSE,
                   null,
                   I18NString.Create("QR-Code settings for ad hoc payments."),
                   CustomData)

        {

            this.Enabled        = Enabled;
            this.URLTemplate    = URLTemplate;
            this.ValidityTime   = ValidityTime;
            this.HashAlgorithm  = HashAlgorithm;
            this.SharedSecret   = SharedSecret;
            this.Length         = Length;
            this.Encoding       = Encoding;
            this.QRCodeQuality  = QRCodeQuality;
            this.Signature      = Signature;


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

                    Description:      I18NString.Create("QR-Code payments controller enabled.")

                )
            );

            #endregion

            #region URLTemplate

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "URLTemplate",
                    ValueGetter:      () => this.URLTemplate,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create("URL template")

                )
            );

            #endregion

            #region ValidityTime

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ValidityTime",
                    ValueGetter:      () => this.ValidityTime?.TotalSeconds.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:   DataTypes.Integer,
                                          Unit:       UnitsOfMeasure.TimeSpan(),
                                          MinLimit:   6
                                      ),

                    Description:      I18NString.Create("The validity time of the time-based one-time passwords.")

                )
            );

            #endregion

            #region HashAlgorithm

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "HashAlgorithm",
                    ValueGetter:      () => this.HashAlgorithm,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList,
                                          ValuesList:  [ "HMAC-SHA256" ]
                                      ),

                    Description:      I18NString.Create("The hash algorithm used for the time-based one-time passwords.")

                )
            );

            #endregion

            #region SharedSecret

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "SharedSecret",
                    ValueGetter:      () => this.SharedSecret,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:   DataTypes.String,
                                          MinLimit:   16
                                      ),

                    Description:      I18NString.Create("The shared secret used for the time-based one-time passwords.")

                )
            );

            #endregion

            #region Length

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Length",
                    ValueGetter:      () => this.Length?.ToString(),

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer
                                      ),

                    Description:      I18NString.Create("The length of the time-based one-time passwords.")

                )
            );

            #endregion

            #region Encoding

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Encoding",
                    ValueGetter:      () => this.Encoding,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create("The allowed characters used for encoding the time-based one-time passwords.")

                )
            );

            #endregion

            #region QRCodeQuality

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "QRCodeQuality",
                    ValueGetter:      () => this.QRCodeQuality,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.MemberList,
                                          ValuesList:  [ "low", "medium", "high" ]
                                      ),

                    Description:      I18NString.Create("The QR-code quality.")

                )
            );

            #endregion

            #region Signature

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Signature",
                    ValueGetter:      () => this.Signature,

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String
                                      ),

                    Description:      I18NString.Create("The digital signature of the QR-code payment URL.")

                )
            );

            #endregion


        }

        #endregion


    }

}
