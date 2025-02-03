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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// Settings for web payments.
    /// </summary>
    public class WebPaymentsCtrlr : APhysicalComponentConfig
    {

        #region Data

        /// <summary>
        /// The default version of the time-based one-time password (TOTP) algorithm.
        /// </summary>
        public const           String    DefaultTOTPVersion          = "v1";


        /// <summary>
        /// The minimal validity time of the time-based one-time password (TOTP) algorithm.
        /// </summary>
        public static readonly TimeSpan  MinimumValidityTime         = TimeSpan.FromSeconds(6);

        /// <summary>
        /// The default validity time of the time-based one-time password (TOTP) algorithm.
        /// </summary>
        public static readonly TimeSpan  DefaultValidityTime         = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The maximum validity time of the time-based one-time password (TOTP) algorithm.
        /// </summary>
        public static readonly TimeSpan  MaximumValidityTime         = TimeSpan.FromHours(1);


        /// <summary>
        /// The minimum length of the shared secret for the time-based one-time password (TOTP) algorithm.
        /// </summary>
        public const           Byte      MinimumSharedSecretLength   = 8;

        /// <summary>
        /// The default length of the shared secret for the time-based one-time password (TOTP) algorithm.
        /// </summary>
        public const           Byte      DefaultSharedSecretLength   = 16;

        /// <summary>
        /// The maximum length of the shared secret for the time-based one-time password (TOTP) algorithm.
        /// </summary>
        public const           Byte      MaximumSharedSecretLength   = 255;


        /// <summary>
        /// The minimum length of the generated time-based one-time password (TOTP).
        /// </summary>
        public const           Byte      MinimumLength               = 6;

        /// <summary>
        /// The default length of the generated time-based one-time password (TOTP).
        /// </summary>
        public const           Byte      DefaultLength               = 12;

        /// <summary>
        /// The maximum length of the generated time-based one-time password (TOTP).
        /// </summary>
        public const           Byte      MaximumLength               = 255;

        #endregion

        #region Properties

        /// <summary>
        /// Whether the web payments controller is enabled.
        /// </summary>
        [Mandatory]
        public Boolean              Enabled          { get; set; }  = false;

        /// <summary>
        /// The URL template.
        /// </summary>
        [Mandatory]
        public URL?                 URLTemplate      { get; set; }

        /// <summary>
        /// The enumeration of supported URL parameters.
        /// </summary>
        [Optional]
        public IEnumerable<String>  URLParameters    { get; }       = [ "TOTP", "maxTime", "maxEnery", "maxCost" ];

        /// <summary>
        /// The supported version of the time-based one-time password (TOTP) algorithm.
        /// </summary>
        [Mandatory]
        public String               TOTPVersion      { get; set; }  = DefaultTOTPVersion;

        /// <summary>
        /// The enumeration of supported time-based one-time password (TOTP) algorithm versions, e.g. "v1"
        /// </summary>
        [Mandatory]
        public IEnumerable<String>  TOTPVersions     { get; }       = [ "v1" ];



        // ChargingStationId



        /// <summary>
        /// The validity time of a TOTP, e.g. 30 seconds.
        /// </summary>
        public TimeSpan             ValidityTime     { get; set; }  = DefaultValidityTime;

        /// <summary>
        /// The shared secret.
        /// </summary>
        [Mandatory]
        public String               SharedSecret     { get; set; }  = RandomExtensions.RandomString(DefaultSharedSecretLength);

        /// <summary>
        /// The length of the TOTP.
        /// </summary>
        [Mandatory]
        public Byte                 Length           { get; set; }  = DefaultLength;

        /// <summary>
        /// The quality of the QR code (Low, Medium, Quartile, High).
        /// </summary>
        [Optional]
        public String?              QRCodeQuality    { get; set; }



        public String?              HashAlgorithm    { get; set; }

        public String?              Encoding         { get; set; }

        public String?              Signature        { get; set; }


        public Boolean              EnableQRCodes    { get; set; }  = false;
        public Boolean              EnableBLE        { get; set; }  = false;
        public Boolean              EnableNFC        { get; set; }  = false;


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new web payments controller.
        /// </summary>
        /// <param name="EVSE">An optional EVSE when component is located at EVSE level, also specifies the connector when component is located at connector level.</param>
        /// <param name="Enabled">web payments controller enabled.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public WebPaymentsCtrlr(EVSE?        EVSE            = null,
                                Boolean?     Enabled         = null,
                                URL?         URLTemplate     = null,
                                String?      TOTPVersion     = DefaultTOTPVersion,

                                // ChargingStationId

                                TimeSpan?    ValidityTime    = null,
                                String?      SharedSecret    = null,
                                Byte?        Length          = null,
                                String?      QRCodeQuality   = null,

                                String?      HashAlgorithm   = null,
                                String?      Encoding        = null,
                                String?      Signature       = null,

                                Boolean?     EnableQRCodes   = null,
                                Boolean?     EnableBLE       = null,
                                Boolean?     EnableNFC       = null,

                                String?      Instance        = null,
                                CustomData?  CustomData      = null)

            : base(nameof(WebPaymentsCtrlr),
                   Instance,
                   EVSE,
                   null,
                   I18NString.Create("Settings for web payments."),
                   CustomData)

        {

            this.Enabled        = Enabled       ?? false;
            this.URLTemplate    = URLTemplate;
            this.TOTPVersion    = TOTPVersion   ?? DefaultTOTPVersion;

            // ChargingStationId

            this.ValidityTime   = ValidityTime  ?? DefaultValidityTime;
            this.SharedSecret   = SharedSecret  ?? RandomExtensions.RandomString(DefaultSharedSecretLength);
            this.Length         = Length        ?? DefaultLength;
            this.QRCodeQuality  = QRCodeQuality;

            this.HashAlgorithm  = HashAlgorithm;
            this.Encoding       = Encoding;
            this.Signature      = Signature;

            this.EnableQRCodes  = EnableQRCodes ?? false;
            this.EnableBLE      = EnableBLE     ?? false;
            this.EnableNFC      = EnableNFC     ?? false;

            #region Parameter checks

            //if (!URLTemplate.HasValue)
            //    throw new ArgumentNullException(nameof(URLTemplate), "The given URL template must not be null!");

            if (!TOTPVersions.Contains(this.TOTPVersion))
                throw new ArgumentException($"The given TOTP version '{TOTPVersion}' is not supported!", nameof(TOTPVersion));

            if (this.ValidityTime < MinimumValidityTime)
                throw new ArgumentException($"The validity time of the TOTP must be at least {MinimumValidityTime.TotalSeconds} seconds!", nameof(ValidityTime));


            if (this.SharedSecret.Length < MinimumSharedSecretLength)
                throw new ArgumentException($"The length of the shared secret for the time-based one-time password algorithm must be at least {MinimumSharedSecretLength} characters!", nameof(SharedSecret));

            if (this.SharedSecret.Length > MaximumSharedSecretLength)
                throw new ArgumentException($"The length of the shared secret for the time-based one-time password algorithm must be at most {MaximumSharedSecretLength} characters!", nameof(SharedSecret));


            if (this.Length < MinimumLength)
                throw new ArgumentException($"The length setting for the generated time-based one-time passwords must be at least {MinimumLength} characters!", nameof(Length));

            if (this.Length > MaximumLength)
                throw new ArgumentException($"The length setting for the generated time-based one-time passwords must be at most {MaximumLength} characters!", nameof(Length));

            #endregion



            #region Enabled

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Enabled",
                    ValueGetter:      () => this.Enabled
                                                ? "true"
                                                : "false",
                    ValueSetter:      (newV, oldV) => {

                                          if (newV == "true")
                                          {
                                              this.Enabled = true;
                                              return new ValueSetterResponse(newV);
                                          }

                                          if (newV == "false")
                                          {
                                              this.Enabled = false;
                                              return new ValueSetterResponse(newV);
                                          }

                                          return new ValueSetterResponse(newV, "Invalid value!");

                                      },

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("Enable the web payments controller and generate time-based one-time password (TOTP) regularly.")

                )
            );

            #endregion

            #region URLTemplate

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "URLTemplate",
                    ValueGetter:      () => this.URLTemplate?.ToString() ?? "",
                    ValueSetter:      (newV, oldV) => {

                                          if (newV.IsNotNullOrEmpty() &&
                                              URL.TryParse(newV, out var newURL))
                                          {

                                              this.URLTemplate = newURL;

                                              return new ValueSetterResponse(newV);

                                          }

                                          return new ValueSetterResponse(
                                                     newV,
                                                     $"Invalid URL template: '{newV}'!"
                                                 );

                                      },

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

            #region TOTPVersion

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "TOTPVersion",
                    ValueGetter:      () => this.TOTPVersion,
                    ValueSetter:      (newV, oldV) => {

                                          if (newV.IsNotNullOrEmpty() &&
                                              TOTPVersions.Contains(newV))
                                          {

                                              this.TOTPVersion = newV;

                                              return new ValueSetterResponse(newV);

                                          }

                                          return new ValueSetterResponse(
                                                     newV,
                                                     $"Invalid TOTP version: '{newV}'!"
                                                 );

                                      },

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer,
                                          Unit:        UnitsOfMeasure.TimeSpan(),
                                          MinLimit:    Convert.ToDecimal(Math.Ceiling(MinimumValidityTime.TotalSeconds))
                                      ),

                    Description:      I18NString.Create("The validity time of the time-based one-time passwords.")

                )
            );

            #endregion


            // ChargingStationId


            #region ValidityTime

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "ValidityTime",
                    ValueGetter:      this.ValidityTime.TotalSeconds.ToString,
                    ValueSetter:      (newV, oldV) => {

                                          if (UInt32.TryParse(newV, out var time))
                                          {

                                              try
                                              {

                                                  var newValidityTime = TimeSpan.FromSeconds(time);

                                                  if (newValidityTime >= MinimumValidityTime)
                                                  {
                                                      this.ValidityTime = newValidityTime;
                                                      return new ValueSetterResponse(newV);
                                                  }

                                                  return new ValueSetterResponse(
                                                             newV,
                                                             $"The validity time of the time-based one-time passwords must be at least {MinimumValidityTime.TotalSeconds} seconds!"
                                                         );

                                              }
                                              catch (Exception e)
                                              {
                                                  return new ValueSetterResponse(
                                                             newV,
                                                             "Invalid validity time of the time-based one-time password: " + e.Message
                                                         );
                                              }

                                          }

                                          return new ValueSetterResponse(
                                                     newV,
                                                     $"Invalid validity time of the time-based one-time password: '{newV}'!"
                                                 );

                                      },

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer,
                                          Unit:        UnitsOfMeasure.TimeSpan(),
                                          MinLimit:    Convert.ToDecimal(Math.Ceiling(MinimumValidityTime.TotalSeconds)),
                                          MaxLimit:    Convert.ToDecimal(Math.Ceiling(MaximumValidityTime.TotalSeconds))
                                      ),

                    Description:      I18NString.Create("The validity time of the time-based one-time passwords.")

                )
            );

            #endregion

            #region SharedSecret

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "SharedSecret",
                    ValueGetter:      () => this.SharedSecret,
                    ValueSetter:      (newV, oldV) => {

                                          if (newV.IsNotNullOrEmpty())
                                          {

                                              if (newV.Length < MinimumSharedSecretLength)
                                                  return new ValueSetterResponse(
                                                             newV,
                                                             $"The length of the shared secret for the time-based one-time password algorithm must be at least {MinimumSharedSecretLength} characters!"
                                                         );

                                              if (newV.Length > MaximumSharedSecretLength)
                                                  return new ValueSetterResponse(
                                                             newV,
                                                             $"The length of the shared secret for the time-based one-time password algorithm must be at most {MaximumSharedSecretLength} characters!"
                                                         );

                                              this.SharedSecret = newV;

                                              return new ValueSetterResponse(newV);

                                          }

                                          return new ValueSetterResponse(
                                                     newV,
                                                     $"Invalid shared secret for the time-based one-time password algorithm: '{newV}'!"
                                                 );

                                      },

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.String,
                                          MinLimit:    MinimumSharedSecretLength,
                                          MaxLimit:    MaximumSharedSecretLength
                                      ),

                    Description:      I18NString.Create("The shared secret used to generate the the time-based one-time passwords.")

                )
            );

            #endregion

            #region Length

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "Length",
                    ValueGetter:      this.Length.ToString,
                    ValueSetter:      (newV, oldV) => {

                                          if (Byte.TryParse(newV, out var length))
                                          {

                                              if (length < MinimumLength)
                                                  return new ValueSetterResponse(
                                                             newV,
                                                             $"The length setting for the generated time-based one-time passwords must be at least {MinimumLength} characters!"
                                                         );

                                              if (length > MaximumLength)
                                                  return new ValueSetterResponse(
                                                             newV,
                                                             $"The length setting for the generated time-based one-time passwords must be at most {MaximumLength} characters!"
                                                         );

                                              this.Length = length;

                                              return new ValueSetterResponse(newV);

                                          }

                                          return new ValueSetterResponse(
                                                     newV,
                                                     $"Invalid length setting for the generated time-based one-time passwords: '{newV}'!"
                                                 );

                    },
                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Integer,
                                          MinLimit:    MinimumLength,
                                          MaxLimit:    MaximumLength
                                      ),

                    Description:      I18NString.Create("The length of the generated time-based one-time passwords.")

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


            #region HashAlgorithm

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "HashAlgorithm",
                    ValueGetter:      () => this.HashAlgorithm,
                    ValueSetter:      (newV, oldV) => {

                                          if (newV == "HMAC-SHA256")
                                          {
                                              this.HashAlgorithm = newV;
                                              return new ValueSetterResponse(newV);
                                          }

                                          return new ValueSetterResponse(newV, "Invalid value!");

                                      },

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


            #region EnableQRCodes

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "EnableQRCodes",
                    ValueGetter:      () => this.EnableQRCodes
                                                ? "true"
                                                : "false",
                    ValueSetter:      (newV, oldV) => {

                                          if (newV == "true")
                                          {
                                              this.EnableQRCodes = true;
                                              return new ValueSetterResponse(newV);
                                          }

                                          if (newV == "false")
                                          {
                                              this.EnableQRCodes = false;
                                              return new ValueSetterResponse(newV);
                                          }

                                          return new ValueSetterResponse(newV, "Invalid value!");

                                      },

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("The battery swap station/EVSE should show QR-Codes using the TOTP algorithm.")

                )
            );

            #endregion

            #region EnableBLE

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "EnableBLE",
                    ValueGetter:      () => this.EnableBLE
                                                ? "true"
                                                : "false",
                    ValueSetter:      (newV, oldV) => {

                                          if (newV == "true")
                                          {
                                              this.EnableBLE = true;
                                              return new ValueSetterResponse(newV);
                                          }

                                          if (newV == "false")
                                          {
                                              this.EnableBLE = false;
                                              return new ValueSetterResponse(newV);
                                          }

                                          return new ValueSetterResponse(newV, "Invalid value!");

                                      },

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("The battery swap station/EVSE should broadcast the generated URL of the time-based one-time password (TOTP) algorithm via Bluetooth Low-Energy beacons.")

                )
            );

            #endregion

            #region EnableNFC

            variableConfigs.Add(
                new VariableConfig(

                    Name:             "EnableNFC",
                    ValueGetter:      () => this.EnableQRCodes
                                                ? "true"
                                                : "false",
                    ValueSetter:      (newV, oldV) => {

                                          if (newV == "true")
                                          {
                                              this.EnableNFC = true;
                                              return new ValueSetterResponse(newV);
                                          }

                                          if (newV == "false")
                                          {
                                              this.EnableNFC = false;
                                              return new ValueSetterResponse(newV);
                                          }

                                          return new ValueSetterResponse(newV, "Invalid value!");

                                      },

                    Attributes:       new VariableAttribute(
                                          Mutability:  MutabilityTypes.ReadWrite
                                      ),

                    Characteristics:  new VariableCharacteristics(
                                          DataType:    DataTypes.Boolean
                                      ),

                    Description:      I18NString.Create("The battery swap station/EVSE should provide the generated URL of the time-based one-time password (TOTP) algorithm via NFC.")

                )
            );

            #endregion

        }

        #endregion



        public JObject ToJSON()
        {

            var json = JSONObject.Create(

                           new JProperty("enabled",        Enabled),
                           // URLTemplate
                           new JProperty("validityTime",   ValidityTime.TotalSeconds),
                           // HashAlgorithm
                           // SharedSecret
                           // Length
                           // Encoding
                           // QRCodeQuality
                           // Signature
                           new JProperty("totp",           ValidityTime)

                       );

            return json;

        }


    }

}
