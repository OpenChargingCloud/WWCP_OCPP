/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// A variable.
    /// </summary>
    public class Variable : ACustomData,
                            IEquatable<Variable>
    {

        #region Properties

        /// <summary>
        /// The case insensitive name of the variable. Name should be taken from the list of standardized variable names whenever possible.
        /// [max 50]
        /// </summary>
        [Mandatory]
        public String   Name        { get; }

        /// <summary>
        /// The optional case insensitive name of the instance in case the variable exists as multiple instances.
        /// [max 50]
        /// </summary>
        [Optional]
        public String?  Instance    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new variable.
        /// </summary>
        /// <param name="Name">The case insensitive name of the variable. Name should be taken from the list of standardized variable names whenever possible.</param>
        /// <param name="Instance">The optional case insensitive name of the instance in case the variable exists as multiple instances.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public Variable(String       Name,
                        String?      Instance     = null,
                        CustomData?  CustomData   = null)

            : base(CustomData)

        {

            this.Name      = Name.     Trim();
            this.Instance  = Instance?.Trim();

            if (this.Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given name must not be null or empty!");

        }

        #endregion


        #region (static) Invalid

        /// <summary>
        /// An invalid variable.
        /// </summary>
        public static Variable Invalid

            => new ("<invalid!>");

        #endregion


        #region Documentation

        // "VariableType": {
        //   "description": "Reference key to a component-variable.\r\n",
        //   "javaType": "Variable",
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

        #region (static) Parse   (JSON, CustomVariableParser = null)

        /// <summary>
        /// Parse the given JSON representation of a variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomVariableParser">A delegate to parse custom variable JSON objects.</param>
        public static Variable Parse(JObject                                 JSON,
                                     CustomJObjectParserDelegate<Variable>?  CustomVariableParser   = null)
        {

            if (TryParse(JSON,
                         out var variable,
                         out var errorResponse,
                         CustomVariableParser))
            {
                return variable!;
            }

            throw new ArgumentException("The given JSON representation of a variable is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Variable, CustomVariableParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Variable">The parsed variable.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject        JSON,
                                       out Variable?  Variable,
                                       out String?    ErrorResponse)

            => TryParse(JSON,
                        out Variable,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a variable.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="Variable">The parsed variable.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomVariableParser">A delegate to parse custom variable JSON objects.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       out Variable?                           Variable,
                                       out String?                             ErrorResponse,
                                       CustomJObjectParserDelegate<Variable>?  CustomVariableParser)
        {

            try
            {

                Variable = default;

                #region Name          [mandatory]

                if (!JSON.ParseMandatoryText("name",
                                             "variable name",
                                             out String Name,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Type          [optional]

                var Instance = JSON.GetString("type");

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Variable = new Variable(Name,
                                        Instance,
                                        CustomData);

                if (CustomVariableParser is not null)
                    Variable = CustomVariableParser(JSON,
                                                    Variable);

                return true;

            }
            catch (Exception e)
            {
                Variable       = default;
                ErrorResponse  = "The given JSON representation of a variable is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomVariableSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomVariableSerializer">A delegate to serialize custom variables.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Variable>?    CustomVariableSerializer     = null,
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

            return CustomVariableSerializer is not null
                       ? CustomVariableSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (Variable1, Variable2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Variable1">A variable.</param>
        /// <param name="Variable2">Another variable.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Variable? Variable1,
                                           Variable? Variable2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(Variable1, Variable2))
                return true;

            // If one is null, but not both, return false.
            if (Variable1 is null || Variable2 is null)
                return false;

            return Variable1.Equals(Variable2);

        }

        #endregion

        #region Operator != (Variable1, Variable2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Variable1">A variable.</param>
        /// <param name="Variable2">Another variable.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Variable? Variable1,
                                           Variable? Variable2)

            => !(Variable1 == Variable2);

        #endregion

        #endregion

        #region IEquatable<Variable> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two variables for equality.
        /// </summary>
        /// <param name="Object">A variable to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Variable variable &&
                   Equals(variable);

        #endregion

        #region Equals(Variable)

        /// <summary>
        /// Compares two variables for equality.
        /// </summary>
        /// <param name="Variable">A variable to compare with.</param>
        public Boolean Equals(Variable? Variable)

            => Variable is not null &&

               String.Equals(Name,     Variable.Name,     StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Instance, Variable.Instance, StringComparison.OrdinalIgnoreCase) &&

               base.  Equals(Variable);

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
