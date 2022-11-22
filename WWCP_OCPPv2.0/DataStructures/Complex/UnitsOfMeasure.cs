/*
 * Copyright (c) 2014-2022 GraphDefined GmbH
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

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A unit of measure and a multiplier.
    /// </summary>
    public class UnitsOfMeasure : ACustomData,
                                  IEquatable<UnitsOfMeasure>
    {

        #region Properties

        /// <summary>
        /// The unit of the measured value. 20
        /// </summary>
        [Mandatory]
        public String  Unit          { get; }

        /// <summary>
        /// Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to the 3rd power.
        /// </summary>
        [Mandatory]
        public Int32   Multiplier    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unit of measure and a multiplier.
        /// </summary>
        /// <param name="Unit">The unit of the measured value.</param>
        /// <param name="Multiplier">Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to the 3rd power.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public UnitsOfMeasure(String       Unit,
                              Int32        Multiplier,
                              CustomData?  CustomData  = null)

            : base(CustomData)

        {

            this.Unit        = Unit.Trim();
            this.Multiplier  = Multiplier;

            if (this.Unit.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Unit), "The given unit of measure must not be null or empty!");

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:UnitsOfMeasureType",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "description": "Represents a UnitOfMeasure with a multiplier\r\n",
        //   "javaType": "UnitsOfMeasure",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "unit": {
        //       "description": "Unit of the value. Default = \"Wh\" if the (default) measurand is an \"Energy\" type.\r\nThis field SHALL use a value from the list Standardized Units of Measurements in Part 2 Appendices. \r\nIf an applicable unit is available in that list, otherwise a \"custom\" unit might be used.\r\n",
        //       "type": "string",
        //       "default": "Wh",
        //       "maxLength": 20
        //     },
        //     "multiplier": {
        //       "description": "Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to the 3rd power. Default is 0.\r\n",
        //       "type": "integer",
        //       "default": 0
        //     }
        //   }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomUnitsOfMeasureParser = null)

        /// <summary>
        /// Parse the given JSON representation of a unit of measure.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomUnitsOfMeasureParser">A delegate to parse custom units of measure.</param>
        public static UnitsOfMeasure Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<UnitsOfMeasure>?  CustomUnitsOfMeasureParser   = null)
        {

            if (TryParse(JSON,
                         out var unitsOfMeasure,
                         out var errorResponse,
                         CustomUnitsOfMeasureParser))
            {
                return unitsOfMeasure!;
            }

            throw new ArgumentException("The given JSON representation of a unit of measure is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out UnitsOfMeasure, out ErrorResponse, CustomUnitsOfMeasureParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a unit of measure.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UnitsOfMeasure">The parsed unit of measure.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out UnitsOfMeasure?  UnitsOfMeasure,
                                       out String?          ErrorResponse)

            => TryParse(JSON,
                        out UnitsOfMeasure,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a unit of measure.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="UnitsOfMeasure">The parsed unit of measure.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnitsOfMeasureParser">A delegate to parse custom units of measure.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out UnitsOfMeasure?                           UnitsOfMeasure,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<UnitsOfMeasure>?  CustomUnitsOfMeasureParser   = null)
        {

            try
            {

                UnitsOfMeasure = default;

                #region Unit          [mandatory]

                if (!JSON.ParseMandatoryText("unit",
                                             "unit measure",
                                             out String Unit,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Multiplier    [mandatory]

                if (!JSON.ParseMandatory("multiplier",
                                         "multiplier",
                                         out Int32 Multiplier,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData    [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                UnitsOfMeasure = new UnitsOfMeasure(Unit.Trim(),
                                                    Multiplier,
                                                    CustomData);

                if (CustomUnitsOfMeasureParser is not null)
                    UnitsOfMeasure = CustomUnitsOfMeasureParser(JSON,
                                                                UnitsOfMeasure);

                return true;

            }
            catch (Exception e)
            {
                UnitsOfMeasure  = default;
                ErrorResponse   = "The given JSON representation of a unit of measure is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomUnitsOfMeasureResponseSerializer = null, CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnitsOfMeasureResponseSerializer">A delegate to serialize custom units of measure.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnitsOfMeasure>?  CustomUnitsOfMeasureResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataResponseSerializer       = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("unit",              Unit),
                           new JProperty("multiplier",        Multiplier),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomUnitsOfMeasureResponseSerializer is not null
                       ? CustomUnitsOfMeasureResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (UnitsOfMeasure1, UnitsOfMeasure2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitsOfMeasure1">Units of measure.</param>
        /// <param name="UnitsOfMeasure2">Other units of measure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UnitsOfMeasure? UnitsOfMeasure1,
                                           UnitsOfMeasure? UnitsOfMeasure2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(UnitsOfMeasure1, UnitsOfMeasure2))
                return true;

            // If one is null, but not both, return false.
            if (UnitsOfMeasure1 is null || UnitsOfMeasure2 is null)
                return false;

            return UnitsOfMeasure1.Equals(UnitsOfMeasure2);

        }

        #endregion

        #region Operator != (UnitsOfMeasure1, UnitsOfMeasure2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UnitsOfMeasure1">Units of measure.</param>
        /// <param name="UnitsOfMeasure2">Other units of measure.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UnitsOfMeasure? UnitsOfMeasure1,
                                           UnitsOfMeasure? UnitsOfMeasure2)

            => !(UnitsOfMeasure1 == UnitsOfMeasure2);

        #endregion

        #endregion

        #region IEquatable<UnitsOfMeasure> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two units of measure for equality.
        /// </summary>
        /// <param name="Object">A unit of measure to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is UnitsOfMeasure unitOfMeasure &&
                   Equals(unitOfMeasure);

        #endregion

        #region Equals(UnitsOfMeasure)

        /// <summary>
        /// Compares two units of measure for equality.
        /// </summary>
        /// <param name="UnitsOfMeasure">A unit of measure to compare with.</param>
        public Boolean Equals(UnitsOfMeasure? UnitsOfMeasure)

            => UnitsOfMeasure is not null &&

               String.    Equals(Unit,
                                 UnitsOfMeasure.Unit,
                                 StringComparison.OrdinalIgnoreCase) &&

               Multiplier.Equals(UnitsOfMeasure.Multiplier) &&

               base.      Equals(UnitsOfMeasure);

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

                return Unit.      GetHashCode() * 5 ^
                       Multiplier.GetHashCode() * 3 ^

                       base.      GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Unit, " ( *10^", Multiplier, " )");

        #endregion

    }

}
