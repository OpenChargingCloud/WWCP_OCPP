﻿/*
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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// An object for reporting components, variables and variable attributes and characteristics.
    /// </summary>
    public class ComponentVariable : ACustomData,
                                     IEquatable<ComponentVariable>
    {

        #region Properties

        /// <summary>
        /// The component for which a report of a variable is requested.
        /// </summary>
        [Mandatory]
        public Component  Component    { get; }

        /// <summary>
        /// The optional variable for which the report is requested.
        /// </summary>
        [Optional]
        public Variable?  Variable     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new additional case insensitive authorization identifier.
        /// </summary>
        /// <param name="Component">A component for which a report of a variable is requested.</param>
        /// <param name="Variable">An optional variable for which the report is requested.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ComponentVariable(Component    Component,
                                 Variable?    Variable     = null,
                                 CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Component  = Component;
            this.Variable   = Variable;

            unchecked
            {

                hashCode = this.Component.GetHashCode()       * 5 ^
                          (this.Variable?.GetHashCode() ?? 0) * 3 ^
                           base.          GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Class to report components, variables and variable attributes and characteristics.\r\n",
        //     "javaType": "ComponentVariable",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "component": {
        //             "$ref": "#/definitions/ComponentType"
        //         },
        //         "variable": {
        //             "$ref": "#/definitions/VariableType"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "component"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomComponentVariableParser = null)

        /// <summary>
        /// Parse the given JSON representation of a component variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomComponentVariableParser">A delegate to parse custom component variable JSON objects.</param>
        public static ComponentVariable Parse(JObject                                          JSON,
                                              CustomJObjectParserDelegate<ComponentVariable>?  CustomComponentVariableParser   = null)
        {

            if (TryParse(JSON,
                         out var componentVariable,
                         out var errorResponse,
                         CustomComponentVariableParser))
            {
                return componentVariable;
            }

            throw new ArgumentException("The given JSON representation of a component variable is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ComponentVariable, CustomComponentVariableParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a component variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ComponentVariable">The parsed component variable.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       [NotNullWhen(true)]  out ComponentVariable?  ComponentVariable,
                                       [NotNullWhen(false)] out String?             ErrorResponse)

            => TryParse(JSON,
                        out ComponentVariable,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a component variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ComponentVariable">The parsed component variable.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomComponentVariableParser">A delegate to parse custom component variable JSON objects.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out ComponentVariable?      ComponentVariable,
                                       [NotNullWhen(false)] out String?                 ErrorResponse,
                                       CustomJObjectParserDelegate<ComponentVariable>?  CustomComponentVariableParser)
        {

            try
            {

                ComponentVariable = default;

                #region Component     [mandatory]

                if (!JSON.ParseMandatoryJSON("component",
                                             "component",
                                             OCPPv2_1.Component.TryParse,
                                             out Component? Component,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Variable      [optional]

                if (JSON.ParseOptionalJSON("variable",
                                           "variable",
                                           OCPPv2_1.Variable.TryParse,
                                           out Variable? Variable,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData    [optional]

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


                ComponentVariable = new ComponentVariable(
                                        Component,
                                        Variable,
                                        CustomData
                                    );

                if (CustomComponentVariableParser is not null)
                    ComponentVariable = CustomComponentVariableParser(JSON,
                                                                      ComponentVariable);

                return true;

            }
            catch (Exception e)
            {
                ComponentVariable  = default;
                ErrorResponse      = "The given JSON representation of a component variable is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomComponentVariableSerializer = null, CustomComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomComponentVariableSerializer">A delegate to serialize custom ComponentVariable objects.</param>
        /// <param name="CustomComponentSerializer">A delegate to serialize custom components.</param>
        /// <param name="CustomEVSESerializer">A delegate to serialize custom EVSEs.</param>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ComponentVariable>?  CustomComponentVariableSerializer   = null,
                              CustomJObjectSerializerDelegate<Component>?          CustomComponentSerializer           = null,
                              CustomJObjectSerializerDelegate<EVSE>?               CustomEVSESerializer                = null,
                              CustomJObjectSerializerDelegate<Variable>?           CustomVariableSerializer            = null,
                              CustomJObjectSerializerDelegate<CustomData>?         CustomCustomDataSerializer          = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("component",    Component. ToJSON(CustomComponentSerializer,
                                                                                 CustomEVSESerializer,
                                                                                 CustomCustomDataSerializer)),

                           Variable is not null
                               ? new JProperty("variable",     Variable.  ToJSON(CustomVariableSerializer,
                                                                                 CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomComponentVariableSerializer is not null
                       ? CustomComponentVariableSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (ComponentVariable1, ComponentVariable2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentVariable1">An component variable.</param>
        /// <param name="ComponentVariable2">Another component variable.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ComponentVariable? ComponentVariable1,
                                           ComponentVariable? ComponentVariable2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ComponentVariable1, ComponentVariable2))
                return true;

            // If one is null, but not both, return false.
            if (ComponentVariable1 is null || ComponentVariable2 is null)
                return false;

            return ComponentVariable1.Equals(ComponentVariable2);

        }

        #endregion

        #region Operator != (ComponentVariable1, ComponentVariable2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ComponentVariable1">An component variable.</param>
        /// <param name="ComponentVariable2">Another component variable.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ComponentVariable? ComponentVariable1,
                                           ComponentVariable? ComponentVariable2)

            => !(ComponentVariable1 == ComponentVariable2);

        #endregion

        #endregion

        #region IEquatable<ComponentVariable> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two component variables for equality.
        /// </summary>
        /// <param name="Object">An component variable to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ComponentVariable componentVariable &&
                   Equals(componentVariable);

        #endregion

        #region Equals(ComponentVariable)

        /// <summary>
        /// Compares two component variables for equality.
        /// </summary>
        /// <param name="ComponentVariable">An component variable to compare with.</param>
        public Boolean Equals(ComponentVariable? ComponentVariable)

            => ComponentVariable is not null &&

               Component.Equals(ComponentVariable.Component) &&

             ((Variable is     null && ComponentVariable.Variable is     null) ||
              (Variable is not null && ComponentVariable.Variable is not null && Variable.Equals(ComponentVariable.Variable))) &&

               base.     Equals(ComponentVariable);

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

                   Component.ToString(),

                   Variable is not null
                       ? $" / {Variable}"
                       : null

               );

        #endregion

    }

}
