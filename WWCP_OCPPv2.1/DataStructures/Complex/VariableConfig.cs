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

    /// <summary>
    /// A variable.
    /// </summary>
    public class VariableConfig : ACustomData,
                                  IEquatable<VariableConfig>
    {

        #region Properties

        /// <summary>
        /// The case insensitive name of the variable. Name should be taken from the list of standardized variable names whenever possible.
        /// [max 50]
        /// </summary>
        [Mandatory]
        public String                                Name               { get; }

        /// <summary>
        /// The optional case insensitive name of the instance in case the variable exists as multiple instances.
        /// [max 50]
        /// </summary>
        [Optional]
        public String?                               Instance           { get; }


        [Optional]
        public IEnumerable<VariableAttribute>        Attributes         { get; }


        [Optional]
        public IEnumerable<VariableCharacteristics>  Characteristics    { get; }


        [Optional]
        public IEnumerable<VariableMonitoring>       Monitorings        { get; }


        [Optional]
        public I18NString?                           Description        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new variable.
        /// </summary>
        /// <param name="Name">The case insensitive name of the variable. Name should be taken from the list of standardized variable names whenever possible.</param>
        /// <param name="Instance">The optional case insensitive name of the instance in case the variable exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public VariableConfig(String                                 Name,
                              String?                                Instance          = null,

                              IEnumerable<VariableAttribute>?        Attributes        = null,
                              IEnumerable<VariableCharacteristics>?  Characteristics   = null,
                              IEnumerable<VariableMonitoring>?       Monitorings       = null,
                              I18NString?                            Description       = null,

                              CustomData?                            CustomData        = null)

            : base(CustomData)

        {

            this.Name             = Name.            Trim();
            this.Instance         = Instance?.       Trim();

            this.Attributes       = Attributes?.     Distinct() ?? [];
            this.Characteristics  = Characteristics?.Distinct() ?? [];
            this.Monitorings      = Monitorings?.    Distinct() ?? [];
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

            => new ("<invalid!>");

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
                                             out String Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type               [optional]

                var Instance = JSON.GetString("type");

                #endregion


                #region Attributes         [optional]

                if (JSON.ParseOptionalHashSet("attributes",
                                              "variable attributes",
                                              VariableAttribute.TryParse,
                                              out HashSet<VariableAttribute> Attributes,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Characteristics    [optional]

                if (JSON.ParseOptionalHashSet("characteristics",
                                              "variable characteristics",
                                              OCPPv2_1.VariableCharacteristics.TryParse,
                                              out HashSet<VariableCharacteristics> Characteristics,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Monitorings        [optional]

                if (JSON.ParseOptionalHashSet("monitorings",
                                              "variable monitorings",
                                              VariableMonitoring.TryParse,
                                              out HashSet<VariableMonitoring> Monitorings,
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
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                VariableConfig = new VariableConfig(

                                     Name,
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
        public JObject ToJSON(CustomJObjectSerializerDelegate<VariableConfig>?    CustomVariableConfigSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
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

                   Instance is not null
                       ? new String(Instance.ToCharArray())
                       : null,

                   Attributes.     Select(variableattribute      => variableattribute.     Clone()),
                   Characteristics.Select(variableCharacteristic => variableCharacteristic.Clone()),
                   Monitorings.    Select(variableMonitoring     => variableMonitoring.    Clone()),

                   Description?.Clone(),

                   CustomData

               );

        #endregion


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
