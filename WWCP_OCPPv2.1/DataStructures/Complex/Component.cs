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
    /// A component.
    /// </summary>
    public class Component : ACustomData,
                             IEquatable<Component>
    {

        #region Properties

        /// <summary>
        /// The case insensitive name of the component. Name should be taken from the list of standardized component names whenever possible.
        /// [max 50]
        /// </summary>
        [Mandatory]
        public String   Name        { get; }

        /// <summary>
        /// The optional case insensitive name of the instance in case the component exists as multiple instances.
        /// [max 50]
        /// </summary>
        [Optional]
        public String?  Instance    { get; }

        /// <summary>
        /// The optional EVSE when component is located at EVSE level, also specifies the connector when component is located at connector level.
        /// </summary>
        [Optional]
        public EVSE?    EVSE        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new component.
        /// </summary>
        /// <param name="Name">The case insensitive name of the component. Name should be taken from the list of standardized component names whenever possible.</param>
        /// <param name="Instance">The optional case insensitive name of the instance in case the component exists as multiple instances.</param>
        /// <param name="EVSE">An optional EVSE when component is located at EVSE level, also specifies the connector when component is located at connector level.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public Component(String       Name,
                         String?      Instance     = null,
                         EVSE?        EVSE         = null,
                         CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Name      = Name.     Trim();
            this.Instance  = Instance?.Trim();
            this.EVSE      = EVSE;

            if (this.Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given name must not be null or empty!");

            if (this.Instance == "")
                this.Instance = null;

            unchecked
            {

                hashCode = this.Name.     ToLower().GetHashCode()       * 7 ^
                          (this.Instance?.ToLower().GetHashCode() ?? 0) * 5 ^
                          (this.EVSE?.              GetHashCode() ?? 0) * 3 ^
                           base.                    GetHashCode();

            }

        }

        #endregion


        #region (static) Invalid

        /// <summary>
        /// An invalid component.
        /// </summary>
        public static Component Invalid

            => new ("<invalid!>");

        #endregion


        #region Documentation

        // "ComponentType": {
        //   "description": "A physical or logical component",
        //   "javaType": "Component",
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

        #region (static) Parse   (JSON, CustomComponentParser = null)

        /// <summary>
        /// Parse the given JSON representation of a component.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomComponentParser">A delegate to parse custom component JSON objects.</param>
        public static Component Parse(JObject                                  JSON,
                                      CustomJObjectParserDelegate<Component>?  CustomComponentParser   = null)
        {

            if (TryParse(JSON,
                         out var component,
                         out var errorResponse,
                         CustomComponentParser))
            {
                return component;
            }

            throw new ArgumentException("The given JSON representation of a component is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Component, CustomComponentParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a component.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Component">The parsed component.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       [NotNullWhen(true)]  out Component?  Component,
                                       [NotNullWhen(false)] out String?     ErrorResponse)

            => TryParse(JSON,
                        out Component,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a component.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Component">The parsed component.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomComponentParser">A delegate to parse custom component JSON objects.</param>
        public static Boolean TryParse(JObject                                  JSON,
                                       [NotNullWhen(true)]  out Component?      Component,
                                       [NotNullWhen(false)] out String?         ErrorResponse,
                                       CustomJObjectParserDelegate<Component>?  CustomComponentParser)
        {

            try
            {

                Component = default;

                #region Name          [mandatory]

                if (!JSON.ParseMandatoryText("name",
                                             "component name",
                                             out var Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Instance      [optional]

                var Instance = JSON.GetString("instance");

                #endregion

                #region EVSE          [optional]

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

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Component = new Component(
                                Name,
                                Instance,
                                EVSE,
                                CustomData
                            );

                if (CustomComponentParser is not null)
                    Component = CustomComponentParser(JSON,
                                                      Component);

                return true;

            }
            catch (Exception e)
            {
                Component      = default;
                ErrorResponse  = "The given JSON representation of a component is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomComponentSerializer = null, CustomEVSESerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Component>?   CustomComponentSerializer    = null,
                              CustomJObjectSerializerDelegate<EVSE>?        CustomEVSESerializer         = null,
                              CustomJObjectSerializerDelegate<CustomData>?  CustomCustomDataSerializer   = null)
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

            return CustomComponentSerializer is not null
                       ? CustomComponentSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Component Clone()

            => new (

                   new String(Name.ToCharArray()),

                   Instance is not null
                       ? new String(Instance.ToCharArray())
                       : null,

                   EVSE?.Clone(),

                   CustomData

               );

        #endregion


        #region Operator overloading

        #region Operator == (Component1, Component2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Component1">A component.</param>
        /// <param name="Component2">Another component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Component? Component1,
                                           Component? Component2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Component1, Component2))
                return true;

            // If one is null, but not both, return false.
            if (Component1 is null || Component2 is null)
                return false;

            return Component1.Equals(Component2);

        }

        #endregion

        #region Operator != (Component1, Component2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Component1">A component.</param>
        /// <param name="Component2">Another component.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Component? Component1,
                                           Component? Component2)

            => !(Component1 == Component2);

        #endregion

        #endregion

        #region IEquatable<Component> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two components for equality.
        /// </summary>
        /// <param name="Object">A component to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Component component &&
                   Equals(component);

        #endregion

        #region Equals(Component)

        /// <summary>
        /// Compares two components for equality.
        /// </summary>
        /// <param name="Component">A component to compare with.</param>
        public Boolean Equals(Component? Component)

            => Component is not null &&

               String.Equals(Name,     Component.Name,     StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Instance, Component.Instance, StringComparison.OrdinalIgnoreCase) &&

             ((EVSE is     null && Component.EVSE is     null) ||
              (EVSE is not null && Component.EVSE is not null && EVSE.Equals(Component.EVSE))) &&

               base.  Equals(Component);

        #endregion

        #endregion

        #region (override) GetHashCode()

        private readonly Int32 hashCode;

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => hashCode;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   Name,

                   Instance is not null && Instance.IsNotNullOrEmpty()
                       ? $" ({Instance})"
                       : null,

                   EVSE is not null
                       ? $" [EVSE {EVSE}]"
                       : null

               );

        #endregion

    }

}
