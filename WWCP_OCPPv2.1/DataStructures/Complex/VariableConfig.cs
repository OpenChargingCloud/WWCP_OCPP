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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    public class ValueSetterResponse(String?  NewValue,
                                     String?  ErrorMessage   = null)
    {

        public String?  NewValue        { get; } = NewValue;
        public String?  ErrorMessage    { get; } = ErrorMessage;

    }


    /// <summary>
    /// A variable.
    /// </summary>
    public class VariableConfig : ACustomData,
                                  IEquatable<VariableConfig>
    {

        #region Data

        public readonly Func<String?>?                                ValueGetter;
        public readonly Func<String?, String?, ValueSetterResponse>?  ValueSetter;

        #endregion

        #region Properties

        /// <summary>
        /// The case insensitive name of the variable. Name should be taken from the list of standardized variable names whenever possible.
        /// [max 50]
        /// </summary>
        [Mandatory]
        public String                    Name               { get; }

        /// <summary>
        /// The optional case insensitive name of the instance in case the variable exists as multiple instances.
        /// [max 50]
        /// </summary>
        [Optional]
        public String?                   Instance           { get; }


        [Optional]
        public VariableAttribute?        Attributes         { get; }


        [Optional]
        public VariableCharacteristics?  Characteristics    { get; }


        [Optional]
        public VariableMonitoring?       Monitorings        { get; }


        [Optional]
        public I18NString?               Description        { get; }


        /// <summary>
        /// The last time this variable was set/updated.
        /// </summary>
        public DateTime?                 LastUpdate         { get; private set; }

        /// <summary>
        /// The value of the variable.
        /// </summary>
        public String?                   Value
            => ValueGetter?.Invoke();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new variable.
        /// </summary>
        /// <param name="Name">The case insensitive name of the variable. Name should be taken from the list of standardized variable names whenever possible.</param>
        /// <param name="ValueGetter">A delegate to return the value of the variable. In some rare cases of WriteOnly variables this might be null!</param>
        /// <param name="ValueSetter">A delegate to set the value of the variable. The old value can optionally be passed as second parameter. This is null for all ReadOnly variables.</param>
        /// 
        /// <param name="Instance">The optional case insensitive name of the instance in case the variable exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public VariableConfig(String                                        Name,
                              Func<String?>?                                ValueGetter       = null,
                              Func<String?, String?, ValueSetterResponse>?  ValueSetter       = null,
                              String?                                       Instance          = null,

                              VariableAttribute?                            Attributes        = null,
                              VariableCharacteristics?                      Characteristics   = null,
                              VariableMonitoring?                           Monitorings       = null,
                              I18NString?                                   Description       = null,

                              CustomData?                                   CustomData        = null)

            : base(CustomData)

        {

            this.Name             = Name.     Trim();
            this.ValueGetter      = ValueGetter;
            this.ValueSetter      = ValueSetter;
            this.Instance         = Instance?.Trim();

            this.Attributes       = Attributes;
            this.Characteristics  = Characteristics;
            this.Monitorings      = Monitorings;
            this.Description      = Description;

            if (this.Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given name must not be null or empty!");

        }

        #endregion


        #region (static) Invalid

        /// <summary>
        /// An invalid variable.
        /// </summary>
        public static VariableConfig Invalid

            => new ("<invalid!>", () => "");

        #endregion


        #region Documentation

        // "VariableConfigType": {
        //   "description": "Reference key to a component-variable.\r\n",
        //   "javaType": "VariableConfig",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "name": {
        //       "description": "Name of the variable. Name should be taken from the list of standardized variable names whenever possible. Case Insensitive. strongly advised to use Camel Case.\r\n",
        //       "type": "string",
        //       "maxLength": 50
        //     },
        //     "instance": {
        //       "description": "Name of instance in case the variable exists as multiple instances. Case Insensitive. strongly advised to use Camel Case.\r\n",
        //       "type": "string",
        //       "maxLength": 50
        //     }
        //   },
        //   "required": [
        //     "name"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomVariableConfigParser = null)

        /// <summary>
        /// Parse the given JSON representation of a variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVariableConfigParser">A delegate to parse custom variable JSON objects.</param>
        public static VariableConfig Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<VariableConfig>?  CustomVariableConfigParser   = null)
        {

            if (TryParse(JSON,
                         out var variable,
                         out var errorResponse,
                         CustomVariableConfigParser) &&
                variable is not null)
            {
                return variable;
            }

            throw new ArgumentException("The given JSON representation of a variable is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out VariableConfig, CustomVariableConfigParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableConfig">The parsed variable.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out VariableConfig?  VariableConfig,
                                       [NotNullWhen(false)] out String?          ErrorResponse)

            => TryParse(JSON,
                        out VariableConfig,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="VariableConfig">The parsed variable.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVariableConfigParser">A delegate to parse custom variable JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out VariableConfig?      VariableConfig,
                                       [NotNullWhen(false)] out String?              ErrorResponse,
                                       CustomJObjectParserDelegate<VariableConfig>?  CustomVariableConfigParser)
        {

            try
            {

                VariableConfig = default;

                #region Name               [mandatory]

                if (!JSON.ParseMandatoryText("name",
                                             "variable name",
                                             out String? Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type               [optional]

                var Instance = JSON.GetString("type");

                #endregion


                #region Attributes         [optional]

                if (JSON.ParseOptionalJSON("attributes",
                                           "variable attributes",
                                           VariableAttribute.TryParse,
                                           out VariableAttribute? Attributes,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Characteristics    [optional]

                if (JSON.ParseOptionalJSON("characteristics",
                                           "variable characteristics",
                                           VariableCharacteristics.TryParse,
                                           out VariableCharacteristics? Characteristics,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Monitorings        [optional]

                if (JSON.ParseOptionalJSON("monitorings",
                                           "variable monitorings",
                                           VariableMonitoring.TryParse,
                                           out VariableMonitoring? Monitorings,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Description        [optional]

                if (JSON.ParseOptional("description",
                                       "variable description",
                                       I18NString.TryParse,
                                       out I18NString? Description,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region CustomData         [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                VariableConfig = new VariableConfig(

                                     Name,
                                     () => "",
                                     null,
                                     Instance,

                                     Attributes,
                                     Characteristics,
                                     Monitorings,
                                     Description,

                                     CustomData

                                 );

                if (CustomVariableConfigParser is not null)
                    VariableConfig = CustomVariableConfigParser(JSON,
                                                                VariableConfig);

                return true;

            }
            catch (Exception e)
            {
                VariableConfig  = default;
                ErrorResponse   = "The given JSON representation of a variable is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVariableConfigSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVariableConfigSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<VariableConfig>?  CustomVariableConfigSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("name",        Name),

                           Instance is not null
                               ? new JProperty("instance",    Instance)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomVariableConfigSerializer is not null
                       ? CustomVariableConfigSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public VariableConfig Clone()

            => new (

                   new String(Name.ToCharArray()),
                   ValueGetter,
                   ValueSetter,

                   Instance is not null
                       ? new String(Instance.ToCharArray())
                       : null,

                   Attributes?.     Clone(),
                   Characteristics?.Clone(),
                   Monitorings?.    Clone(),

                   Description?.    Clone(),

                   CustomData

               );

        #endregion


        public ValueSetterResponse Set(String? NewValue,
                                       String? OldValue = null)

        {

            var response = ValueSetter?.Invoke(NewValue,
                                               OldValue)

                               ?? new ValueSetterResponse(
                                      "error",
                                      "Internal error!"
                                  );

            if (response.ErrorMessage is null)
                LastUpdate = Timestamp.Now;

            return response;

        }



        #region Operator overloading

        #region Operator == (VariableConfig1, VariableConfig2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableConfig1">A variable.</param>
        /// <param name="VariableConfig2">Another variable.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (VariableConfig? VariableConfig1,
                                           VariableConfig? VariableConfig2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(VariableConfig1, VariableConfig2))
                return true;

            // If one is null, but not both, return false.
            if (VariableConfig1 is null || VariableConfig2 is null)
                return false;

            return VariableConfig1.Equals(VariableConfig2);

        }

        #endregion

        #region Operator != (VariableConfig1, VariableConfig2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VariableConfig1">A variable.</param>
        /// <param name="VariableConfig2">Another variable.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (VariableConfig? VariableConfig1,
                                           VariableConfig? VariableConfig2)

            => !(VariableConfig1 == VariableConfig2);

        #endregion

        #endregion

        #region IEquatable<VariableConfig> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two variables for equality.
        /// </summary>
        /// <param name="Object">A variable to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is VariableConfig variable &&
                   Equals(variable);

        #endregion

        #region Equals(VariableConfig)

        /// <summary>
        /// Compares two variables for equality.
        /// </summary>
        /// <param name="VariableConfig">A variable to compare with.</param>
        public Boolean Equals(VariableConfig? VariableConfig)

            => VariableConfig is not null &&

               String.Equals(Name,     VariableConfig.Name,     StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Instance, VariableConfig.Instance, StringComparison.OrdinalIgnoreCase) &&

               base.  Equals(VariableConfig);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return Name.     ToLower().GetHashCode()       * 5 ^
                      (Instance?.ToLower().GetHashCode() ?? 0) * 3 ^

                       base.             GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Name,

                   Instance is not null && Instance.IsNotNullOrEmpty()
                       ? " (" + Instance + ")"
                       : null

               );

        #endregion

    }

}
