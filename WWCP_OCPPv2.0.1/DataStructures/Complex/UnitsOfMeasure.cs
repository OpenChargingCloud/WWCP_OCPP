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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0_1
{

    /// <summary>
    /// A unit of measure and a multiplier.
    /// </summary>
    public class UnitsOfMeasure : ACustomData,
                                  IEquatable<UnitsOfMeasure>
    {

        #region Properties

        /// <summary>
        /// The unit of the measured value.
        /// </summary>
        [Optional]
        public UnitOfMeasure?  Unit          { get; }

        /// <summary>
        /// Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to the 3rd power.
        /// </summary>
        [Optional]
        public Int32?          Multiplier    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unit of measure and a multiplier.
        /// </summary>
        /// <param name="Unit">The unit of the measured value.</param>
        /// <param name="Multiplier">Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to the 3rd power.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public UnitsOfMeasure(UnitOfMeasure?  Unit         = null,
                              Int32?          Multiplier   = null,
                              CustomData?     CustomData   = null)

            : base(CustomData)

        {

            this.Unit        = Unit;
            this.Multiplier  = Multiplier;

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
        /// <param name="CustomUnitsOfMeasureParser">An optional delegate to parse custom units of measure.</param>
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
        /// <param name="CustomUnitsOfMeasureParser">An optional delegate to parse custom units of measure.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out UnitsOfMeasure?                           UnitsOfMeasure,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<UnitsOfMeasure>?  CustomUnitsOfMeasureParser   = null)
        {

            try
            {

                UnitsOfMeasure = default;

                #region Unit          [optional]

                if (JSON.ParseOptional("unit",
                                       "unit measure",
                                       UnitOfMeasure.TryParse,
                                       out UnitOfMeasure? Unit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Multiplier    [optional]

                if (!JSON.ParseOptional("multiplier",
                                        "multiplier",
                                        out Int32? Multiplier,
                                        out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

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


                UnitsOfMeasure = new UnitsOfMeasure(
                                     Unit,
                                     Multiplier,
                                     CustomData
                                 );

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

        #region ToJSON(CustomUnitsOfMeasureSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomUnitsOfMeasureSerializer">A delegate to serialize custom units of measure.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<UnitsOfMeasure>?  CustomUnitsOfMeasureSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                           Unit.      HasValue
                               ? new JProperty("unit",         Unit.      ToString())
                               : null,

                           Multiplier.HasValue
                               ? new JProperty("multiplier",   Multiplier)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomUnitsOfMeasureSerializer is not null
                       ? CustomUnitsOfMeasureSerializer(this, json)
                       : json;

        }

        #endregion


        #region Static Definitions

        /// <summary>
        /// Degrees (temperature).
        /// </summary>
        public static UnitsOfMeasure Celsius(Int32        Multiplier,
                                             CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Celsius,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// Degrees (temperature).
        /// </summary>
        public static UnitsOfMeasure Fahrenheit(Int32        Multiplier,
                                                CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Fahrenheit,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// Watt-hours (energy).
        /// </summary>
        public static UnitsOfMeasure Wh(Int32        Multiplier,
                                        CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Wh,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// kiloWatt-hours (energy).
        /// </summary>
        public static UnitsOfMeasure kWh(Int32        Multiplier,
                                         CustomData?  CustomData   = null)

            => new (UnitOfMeasure.kWh,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// Var-hours (reactive energy).
        /// </summary>
        public static UnitsOfMeasure varh(Int32        Multiplier,
                                          CustomData?  CustomData   = null)

            => new (UnitOfMeasure.varh,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// kilovar-hours (reactive energy).
        /// </summary>
        public static UnitsOfMeasure kvarh(Int32        Multiplier,
                                           CustomData?  CustomData   = null)

            => new (UnitOfMeasure.kvarh,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// Watts (power).
        /// </summary>
        public static UnitsOfMeasure Watts(Int32        Multiplier,
                                           CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Watts,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// kiloWatts (power).
        /// </summary>
        public static UnitsOfMeasure kW(Int32        Multiplier,
                                        CustomData?  CustomData   = null)

            => new (UnitOfMeasure.kW,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// VoltAmpere (apparent power).
        /// </summary>
        public static UnitsOfMeasure VoltAmpere(Int32        Multiplier,
                                                CustomData?  CustomData   = null)

            => new (UnitOfMeasure.VoltAmpere,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// kiloVolt Ampere (apparent power).
        /// </summary>
        public static UnitsOfMeasure kVA(Int32        Multiplier,
                                         CustomData?  CustomData   = null)

            => new (UnitOfMeasure.kVA,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// Vars (reactive power).
        /// </summary>
        public static UnitsOfMeasure var(Int32        Multiplier,
                                         CustomData?  CustomData   = null)

            => new (UnitOfMeasure.var,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// kilovars (reactive power).
        /// </summary>
        public static UnitsOfMeasure kvar(Int32        Multiplier,
                                          CustomData?  CustomData   = null)

            => new (UnitOfMeasure.kvar,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// Amperes (current).
        /// </summary>
        public static UnitsOfMeasure Amperes(Int32        Multiplier,
                                             CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Amperes,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// Voltage (r.m.s. AC).
        /// </summary>
        public static UnitsOfMeasure Voltage(Int32        Multiplier,
                                             CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Voltage,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// Degrees Kelvin (temperature).
        /// </summary>
        public static UnitsOfMeasure Kelvin(Int32        Multiplier,
                                            CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Kelvin,
                    Multiplier,
                    CustomData);

        /// <summary>
        /// Percentage.
        /// </summary>
        public static UnitsOfMeasure Percent(Int32        Multiplier,
                                             CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Percent,
                    Multiplier,
                    CustomData);

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

            => Object is UnitsOfMeasure unitsOfMeasure &&
                   Equals(unitsOfMeasure);

        #endregion

        #region Equals(UnitsOfMeasure)

        /// <summary>
        /// Compares two units of measure for equality.
        /// </summary>
        /// <param name="UnitsOfMeasure">A unit of measure to compare with.</param>
        public Boolean Equals(UnitsOfMeasure? UnitsOfMeasure)

            => UnitsOfMeasure is not null &&

            ((!Unit.      HasValue && UnitsOfMeasure.Unit.      HasValue) ||
              (Unit.      HasValue && UnitsOfMeasure.Unit.      HasValue && Unit.      Equals(UnitsOfMeasure.Unit)))       &&

            ((!Multiplier.HasValue && UnitsOfMeasure.Multiplier.HasValue) ||
              (Multiplier.HasValue && UnitsOfMeasure.Multiplier.HasValue && Multiplier.Equals(UnitsOfMeasure.Multiplier))) &&

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

                return (Unit?.      GetHashCode() ?? 0) * 5 ^
                       (Multiplier?.GetHashCode() ?? 0) * 3 ^

                        base.       GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Unit?.ToString() ?? "-"} ( *10^{Multiplier?.ToString() ?? "-"} )";

        #endregion

    }

}
