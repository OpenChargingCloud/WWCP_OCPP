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

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A component configuration.
    /// </summary>
    public class ComponentConfig : ACustomData,
                                   IEquatable<ComponentConfig>
    {

        #region Properties

        /// <summary>
        /// The case insensitive name of the component. Name should be taken from the list of standardized component names whenever possible.
        /// [max 50]
        /// </summary>
        [Mandatory]
        public String                       Name               { get; }

        /// <summary>
        /// The optional case insensitive name of the instance in case the component exists as multiple instances.
        /// [max 50]
        /// </summary>
        [Optional]
        public String?                      Instance           { get; }

        /// <summary>
        /// The optional EVSE when component is located at EVSE level, also specifies the connector when component is located at connector level.
        /// </summary>
        [Optional]
        public EVSE?                        EVSE               { get; }


        protected readonly List<VariableConfig> variableConfigs = [];

        [Optional]
        public IEnumerable<VariableConfig>  VariableConfigs
            => variableConfigs;


        [Optional]
        public I18NString?                  Description        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new component.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component. Name should be taken from the list of standardized component names whenever possible.</param>
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="EVSE">An optional EVSE when component is located at EVSE level, also specifies the connector when component is located at connector level.</param>
        /// <param name="VariableConfigs">An optional enumeration of variable configurations.</param>
        /// <param name="Description">An optional multi-language description of the component.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ComponentConfig(String                        Name,
                               String?                       Instance          = null,
                               EVSE?                         EVSE              = null,

                               IEnumerable<VariableConfig>?  VariableConfigs   = null,
                               I18NString?                   Description       = null,

                               CustomData?                   CustomData        = null)

            : base(CustomData)

        {

            this.Name             = Name.     Trim();
            this.Instance         = Instance?.Trim();
            this.EVSE             = EVSE;

            this.variableConfigs.AddRange(VariableConfigs?.Distinct() ?? []);
            this.Description      = Description;

            if (this.Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given name must not be null or empty!");

        }

        #endregion


        #region (static) Invalid

        /// <summary>
        /// An invalid component.
        /// </summary>
        public static ComponentConfig Invalid

            => new ("<invalid!>");

        #endregion


        #region Documentation

        // "ComponentConfigType": {
        //   "description": "A physical or logical component",
        //   "javaType": "ComponentConfig",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "evse": {
        //       "$ref": "#/definitions/EVSEType"
        //     },
        //     "name": {
        //       "description": "Name of the component. Name should be taken from the list of standardized component names whenever possible. Case Insensitive. strongly advised to use Camel Case.",
        //       "type":        "string",
        //       "maxLength":    50
        //     },
        //     "instance": {
        //       "description": "Name of instance in case the component exists as multiple instances. Case Insensitive. strongly advised to use Camel Case.",
        //       "type":        "string",
        //       "maxLength":    50
        //     }
        //   },
        //   "required": [
        //     "name"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomComponentConfigParser = null)

        /// <summary>
        /// Parse the given JSON representation of a component.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomComponentConfigParser">A delegate to parse custom component JSON objects.</param>
        public static ComponentConfig Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<ComponentConfig>?  CustomComponentConfigParser   = null)
        {

            if (TryParse(JSON,
                         out var component,
                         out var errorResponse,
                         CustomComponentConfigParser) &&
                component is not null)
            {
                return component;
            }

            throw new ArgumentException("The given JSON representation of a component is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ComponentConfig, CustomComponentConfigParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a component.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ComponentConfig">The parsed component.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out ComponentConfig?  ComponentConfig,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParse(JSON,
                        out ComponentConfig,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a component.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ComponentConfig">The parsed component.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomComponentConfigParser">A delegate to parse custom component JSON objects.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out ComponentConfig?      ComponentConfig,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<ComponentConfig>?  CustomComponentConfigParser)
        {

            try
            {

                ComponentConfig = default;

                #region Name               [mandatory]

                if (!JSON.ParseMandatoryText("name",
                                             "component name",
                                             out var Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type               [optional]

                var Instance = JSON.GetString("type");

                #endregion

                #region EVSE               [optional]

                if (JSON.ParseOptionalJSON("evse",
                                           "custom data",
                                           OCPPv2_1.EVSE.TryParse,
                                           out EVSE? EVSE,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region VariableConfigs    [optional]

                if (JSON.ParseOptionalHashSet("variableConfigs",
                                              "variable configs",
                                              VariableConfig.TryParse,
                                              out HashSet<VariableConfig> VariableConfigs,
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                ComponentConfig = new ComponentConfig(

                                      Name,
                                      Instance,
                                      EVSE,

                                      VariableConfigs,
                                      Description,

                                      CustomData

                                  );

                if (CustomComponentConfigParser is not null)
                    ComponentConfig = CustomComponentConfigParser(JSON,
                                                                  ComponentConfig);

                return true;

            }
            catch (Exception e)
            {
                ComponentConfig  = default;
                ErrorResponse    = "The given JSON representation of a component is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomComponentConfigSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomComponentConfigSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ComponentConfig>?  CustomComponentConfigSerializer   = null,
                              CustomJObjectSerializerDelegate<EVSE>?             CustomEVSESerializer              = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataSerializer        = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("name",         Name),

                           Instance is not null
                               ? new JProperty("instance",     Instance)
                               : null,

                           EVSE is not null
                               ? new JProperty("evse",         EVSE.      ToJSON(CustomEVSESerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomComponentConfigSerializer is not null
                       ? CustomComponentConfigSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public ComponentConfig Clone()

            => new (

                   new String(Name.ToCharArray()),

                   Instance is not null
                       ? new String(Instance.ToCharArray())
                       : null,

                   EVSE?.Clone(),

                   VariableConfigs.Select(variableConfig => variableConfig.Clone()),

                   Description?.Clone(),

                   CustomData

               );

        #endregion


        #region Operator overloading

        #region Operator == (ComponentConfig1, ComponentConfig2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentConfig1">A component.</param>
        /// <param name="ComponentConfig2">Another component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ComponentConfig? ComponentConfig1,
                                           ComponentConfig? ComponentConfig2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ComponentConfig1, ComponentConfig2))
                return true;

            // If one is null, but not both, return false.
            if (ComponentConfig1 is null || ComponentConfig2 is null)
                return false;

            return ComponentConfig1.Equals(ComponentConfig2);

        }

        #endregion

        #region Operator != (ComponentConfig1, ComponentConfig2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentConfig1">A component.</param>
        /// <param name="ComponentConfig2">Another component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ComponentConfig? ComponentConfig1,
                                           ComponentConfig? ComponentConfig2)

            => !(ComponentConfig1 == ComponentConfig2);

        #endregion

        #endregion

        #region IEquatable<ComponentConfig> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two components for equality.
        /// </summary>
        /// <param name="Object">A component to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ComponentConfig component &&
                   Equals(component);

        #endregion

        #region Equals(ComponentConfig)

        /// <summary>
        /// Compares two components for equality.
        /// </summary>
        /// <param name="ComponentConfig">A component to compare with.</param>
        public Boolean Equals(ComponentConfig? ComponentConfig)

            => ComponentConfig is not null &&

               String.Equals(Name,     ComponentConfig.Name,     StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Instance, ComponentConfig.Instance, StringComparison.OrdinalIgnoreCase) &&

             ((EVSE is     null && ComponentConfig.EVSE is     null) ||
              (EVSE is not null && ComponentConfig.EVSE is not null && EVSE.Equals(ComponentConfig.EVSE))) &&

               base.  Equals(ComponentConfig);

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

                return Name.     ToLower().GetHashCode()       * 7 ^
                      (Instance?.ToLower().GetHashCode() ?? 0) * 5 ^
                      (EVSE?.              GetHashCode() ?? 0) * 3 ^

                       base.               GetHashCode();

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
                       : null,

                   EVSE is not null
                       ? " [" + EVSE.ToString() + "]"
                       : null

               );

        #endregion

    }

}
