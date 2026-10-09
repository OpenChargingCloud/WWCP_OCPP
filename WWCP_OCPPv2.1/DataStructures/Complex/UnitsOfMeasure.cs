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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A unit of measure and a multiplier.
    /// </summary>
    /// <param name="Unit">The unit of the measured value.</param>
    /// <param name="Multiplier">Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to the 3rd power.</param>
    /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
    public class UnitsOfMeasure(UnitOfMeasure  Unit,
                                Int32?         Multiplier   = null,
                                CustomData?    CustomData   = null) : ACustomData(CustomData),
                                                                      IEquatable<UnitsOfMeasure>,
                                                                      ICBORSerializable<UnitsOfMeasure>
    {

        #region Properties

        /// <summary>
        /// The unit of the measured value.
        /// </summary>
        [Mandatory]
        public UnitOfMeasure  Unit          { get; } = Unit;

        /// <summary>
        /// Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to the 3rd power.
        /// Default: 0 (10^0 == *1).
        /// </summary>
        [Optional]
        public Int32          Multiplier    { get; } = Multiplier ?? 0;

        #endregion


        #region Documentation

        // {
        //     "description": "Represents a UnitOfMeasure with a multiplier",
        //     "javaType": "UnitOfMeasure",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "unit": {
        //             "description": "Unit of the value. Default = \"Wh\" if the (default) measurand is an \"Energy\" type.
        //                             This field SHALL use a value from the list Standardized Units of Measurements in Part 2 Appendices.
        //                             If an applicable unit is available in that list, otherwise a \"custom\" unit might be used.",
        //             "type": "string",
        //             "default": "Wh",
        //             "maxLength": 20
        //         },
        //         "multiplier": {
        //             "description": "Multiplier, this value represents the exponent to base 10. I.e. multiplier 3 means 10 raised to
        //                             the 3rd power. Default is 0.
        //                             The _multiplier_ only multiplies the value of the measurand.
        //                             It does not specify a conversion between units, for example, kW and W.",
        //             "type": "integer",
        //             "default": 0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
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
                return unitsOfMeasure;
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
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out UnitsOfMeasure?  UnitsOfMeasure,
                                       [NotNullWhen(false)] out String?          ErrorResponse)

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
                                       [NotNullWhen(true)]  out UnitsOfMeasure?      UnitsOfMeasure,
                                       [NotNullWhen(false)] out String?              ErrorResponse,
                                       CustomJObjectParserDelegate<UnitsOfMeasure>?  CustomUnitsOfMeasureParser   = null)
        {

            try
            {

                UnitsOfMeasure = default;

                #region Unit          [optional]

                // Optional in OCPP 2.1, "Wh" where it is absent. It was required.
                if (JSON.ParseOptional("unit",
                                       "unit measure",
                                       UnitOfMeasure.TryParse,
                                       out UnitOfMeasure Unit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }
                else
                    Unit = UnitOfMeasure.Wh;

                #endregion

                #region Multiplier    [optional]

                if (JSON.ParseOptional("multiplier",
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
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

                                 new JProperty("unit",         Unit.      ToString()),

                           Multiplier != 0
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

        #region (static) TryParseCBOR(CBOR, out UnitsOfMeasure, out ErrorResponse, CustomUnitsOfMeasureParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of an unit of measure.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="UnitsOfMeasure">The unit of measure.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out UnitsOfMeasure?  UnitsOfMeasure,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out UnitsOfMeasure,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of an unit of measure.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="UnitsOfMeasure">The unit of measure.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomUnitsOfMeasureParser">An optional delegate to read custom unit of measures.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out UnitsOfMeasure?           UnitsOfMeasure,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<UnitsOfMeasure>?  CustomUnitsOfMeasureParser)
        {

            try
            {

                UnitsOfMeasure = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of an unit of measure is not a map!";
                    return false;
                }

                OCPPv2_1.UnitOfMeasure? Unit = null;

                if (CBOR.ParseOptionalText("unit",
                                           "unit of measure",
                                           out var UnitText,
                                           out ErrorResponse))
                {

                    if (!OCPPv2_1.UnitOfMeasure.TryParse(UnitText!, out var UnitValue))
                    {
                        ErrorResponse = $"Invalid unit of measure '{UnitText}'!";
                        return false;
                    }

                    Unit = UnitValue;

                }

                if (ErrorResponse is not null)
                    return false;


                Int32? Multiplier = null;

                if (CBOR.ParseOptionalInt64("multiplier",
                                            "multiplier",
                                            out var multiplier,
                                            out ErrorResponse))
                {

                    if (multiplier is not Int64 multiplierValue || multiplierValue < Int32.MinValue || multiplierValue > Int32.MaxValue)
                    {
                        ErrorResponse = $"Invalid multiplier '{multiplier}'!";
                        return false;
                    }

                    Multiplier = (Int32) multiplierValue;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                UnitsOfMeasure = new UnitsOfMeasure(
                                     Unit ?? OCPPv2_1.UnitOfMeasure.Wh,
                                     Multiplier,
                                     CustomData
                                 );

                if (CustomUnitsOfMeasureParser is not null)
                    UnitsOfMeasure = CustomUnitsOfMeasureParser(CBOR,
                                                UnitsOfMeasure);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                UnitsOfMeasure  = default;
                ErrorResponse  = "The given CBOR representation of an unit of measure is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<UnitsOfMeasure>.TryParse(CBOR, out UnitsOfMeasure, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of an unit of measure - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<UnitsOfMeasure>.TryParse(CBORValue                         CBOR,
                                                                out UnitsOfMeasure                  Value,
                                                                [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomUnitsOfMeasureSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this unit of measure: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomUnitsOfMeasureSerializer">A delegate to serialize custom unit of measures.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<UnitsOfMeasure>? CustomUnitsOfMeasureSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("unit",                    CBORValue.FromText(Unit.ToString())),
                           ("multiplier",              OCPPCBORExtensions.Int(Multiplier != 0 ? Multiplier : null)),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomUnitsOfMeasureSerializer is not null
                       ? CustomUnitsOfMeasureSerializer(this, cbor)
                       : cbor;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this unit of measure and a multiplier.
        /// </summary>
        public UnitsOfMeasure Clone()

            => new (
                   Unit.Clone(),
                   Multiplier,
                   CustomData
               );

        #endregion

        #region Metrology: TryToMetrologicalValue(Value, out MetrologicalValue), TryFromMetrologicalValue(...)

        /// <summary>
        /// The units of the specification's "Standardized Units of Measure" that are
        /// metrological units: the unit and the power of ten its prefix is.
        /// </summary>
        private static readonly Dictionary<String, (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure Unit, Int32 Exponent)> metrologicalUnits = new() {
            { "A",        (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Ampere,                  0) },
            { "B",        (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Byte,                    0) },
            { "Celsius",  (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Celsius,                 0) },
            { "Deg",      (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Degree,                  0) },
            { "Hz",       (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Hertz,                   0) },
            { "mHz",      (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Hertz,                  -3) },
            { "K",        (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Kelvin,                  0) },
            { "lx",       (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Lux,                     0) },
            { "m",        (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Meter,                   0) },
            { "N",        (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Newton,                  0) },
            { "Ohm",      (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Ohm,                     0) },
            { "kPa",      (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Pascal,                  3) },
            { "Percent",  (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Percent,                 0) },
            { "s",        (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Second,                  0) },
            { "V",        (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Volt,                    0) },
            { "VA",       (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.VoltAmpere,              0) },
            { "kVA",      (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.VoltAmpere,              3) },
            { "var",      (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.VoltAmpereReactive,      0) },
            { "kvar",     (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.VoltAmpereReactive,      3) },
            { "varh",     (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.VoltAmpereReactiveHour,  0) },
            { "kvarh",    (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.VoltAmpereReactiveHour,  3) },
            { "W",        (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Watt,                    0) },
            { "kW",       (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.Watt,                    3) },
            { "Wh",       (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.WattHour,                0) },
            { "kWh",      (org.GraphDefined.Vanaheimr.Illias.UnitOfMeasure.WattHour,                3) }
        };


        /// <summary>
        /// The given value in this unit and with this multiplier as a metrological
        /// value - for every unit of the specification that is a metrological unit,
        /// and as long as no custom data comes with it.
        /// </summary>
        /// <param name="Value">A value in this unit and with this multiplier.</param>
        /// <param name="MetrologicalValue">The metrological value.</param>
        public Boolean TryToMetrologicalValue(Decimal                Value,
                                              out MetrologicalValue  MetrologicalValue)
        {

            MetrologicalValue = default;

            if (CustomData is not null ||
                !metrologicalUnits.TryGetValue(Unit.ToString(), out var unit) ||
                !SIPrefix.TryFrom(unit.Exponent + Multiplier, out var prefix))
            {
                return false;
            }

            MetrologicalValue = new MetrologicalValue(Value, unit.Unit, prefix);
            return true;

        }


        /// <summary>
        /// The value, the unit and the multiplier of the given metrological value -
        /// the unit the specification names with a k where there is one, and no unit
        /// at all for watt-hours without a prefix, the default of a sampled value.
        /// </summary>
        /// <param name="MetrologicalValue">A metrological value.</param>
        /// <param name="Value">The value.</param>
        /// <param name="UnitsOfMeasure">The unit and the multiplier, or null for Wh.</param>
        public static Boolean TryFromMetrologicalValue(MetrologicalValue    MetrologicalValue,
                                                       out Decimal          Value,
                                                       out UnitsOfMeasure?  UnitsOfMeasure)
        {

            Value           = MetrologicalValue.Value;
            UnitsOfMeasure  = null;

            var exponent    = MetrologicalValue.Prefix.Exponent;

            // Of the units with the same metrological unit: the one whose prefix
            // is the value's (mHz for 10^-3, kWh for 10^3), else the largest one
            // not above it that has no fraction of a prefix (kWh for 10^6), else
            // the unit itself (Hz for 10^-1), else whichever there is (kPa).
            var candidates  = metrologicalUnits.Where(entry => MetrologicalValue.Unit == entry.Value.Unit).
                                                OrderByDescending(entry => entry.Value.Exponent).
                                                ToArray();

            if (candidates.Length == 0)
                return false;

            var chosen      = candidates.FirstOrDefault(entry => entry.Value.Exponent == exponent);

            if (chosen.Key is null)
                chosen      = candidates.FirstOrDefault(entry => entry.Value.Exponent >= 0 && entry.Value.Exponent <= exponent);

            if (chosen.Key is null)
                chosen      = candidates.FirstOrDefault(entry => entry.Value.Exponent == 0);

            if (chosen.Key is null)
                chosen      = candidates.Last();

            if (chosen.Key == "Wh" && exponent == 0)
                return true;

            UnitsOfMeasure  = new UnitsOfMeasure(
                                  OCPPv2_1.UnitOfMeasure.Parse(chosen.Key),
                                  exponent - chosen.Value.Exponent
                              );

            return true;

        }

        #endregion


        #region Static Definitions

        /// <summary>
        /// Degrees (temperature).
        /// </summary>
        public static UnitsOfMeasure Celsius(Int32        Multiplier   = 0,
                                             CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Celsius,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// Degrees (temperature).
        /// </summary>
        public static UnitsOfMeasure Fahrenheit(Int32        Multiplier   = 0,
                                                CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Fahrenheit,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// Watt-hours (energy).
        /// </summary>
        public static UnitsOfMeasure Wh(Int32        Multiplier   = 0,
                                        CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Wh,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// kiloWatt-hours (energy).
        /// </summary>
        public static UnitsOfMeasure kWh(Int32        Multiplier   = 0,
                                         CustomData?  CustomData   = null)

            => new (UnitOfMeasure.kWh,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// Var-hours (reactive energy).
        /// </summary>
        public static UnitsOfMeasure varh(Int32        Multiplier   = 0,
                                          CustomData?  CustomData   = null)

            => new (UnitOfMeasure.varh,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// kilovar-hours (reactive energy).
        /// </summary>
        public static UnitsOfMeasure kvarh(Int32        Multiplier   = 0,
                                           CustomData?  CustomData   = null)

            => new (UnitOfMeasure.kvarh,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// Watts (power).
        /// </summary>
        public static UnitsOfMeasure Watts(Int32        Multiplier   = 0,
                                           CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Watts,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// kiloWatts (power).
        /// </summary>
        public static UnitsOfMeasure kW(Int32        Multiplier   = 0,
                                        CustomData?  CustomData   = null)

            => new (UnitOfMeasure.kW,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// VoltAmpere (apparent power).
        /// </summary>
        public static UnitsOfMeasure VoltAmpere(Int32        Multiplier   = 0,
                                                CustomData?  CustomData   = null)

            => new (UnitOfMeasure.VoltAmpere,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// kiloVolt Ampere (apparent power).
        /// </summary>
        public static UnitsOfMeasure kVA(Int32        Multiplier   = 0,
                                         CustomData?  CustomData   = null)

            => new (UnitOfMeasure.kVA,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// Vars (reactive power).
        /// </summary>
        public static UnitsOfMeasure var(Int32        Multiplier   = 0,
                                         CustomData?  CustomData   = null)

            => new (UnitOfMeasure.var,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// kilovars (reactive power).
        /// </summary>
        public static UnitsOfMeasure kvar(Int32        Multiplier   = 0,
                                          CustomData?  CustomData   = null)

            => new (UnitOfMeasure.kvar,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// Amperes (current).
        /// </summary>
        public static UnitsOfMeasure Amperes(Int32        Multiplier   = 0,
                                             CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Amperes,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// Voltage (r.m.s. AC).
        /// </summary>
        public static UnitsOfMeasure Voltage(Int32        Multiplier   = 0,
                                             CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Voltage,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// Degrees Kelvin (temperature).
        /// </summary>
        public static UnitsOfMeasure Kelvin(Int32        Multiplier   = 0,
                                            CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Kelvin,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// Percentage.
        /// </summary>
        public static UnitsOfMeasure Percent(Int32        Multiplier   = 0,
                                             CustomData?  CustomData   = null)

            => new (UnitOfMeasure.Percent,
                    Multiplier,
                    CustomData);


        /// <summary>
        /// TimeSpan (default: Seconds).
        /// </summary>
        public static UnitsOfMeasure TimeSpan(Int32        Multiplier   = 0,
                                              CustomData?  CustomData   = null)

            => new (UnitOfMeasure.TimeSpan,
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

               Unit.      Equals(UnitsOfMeasure.Unit)       &&
               Multiplier.Equals(UnitsOfMeasure.Multiplier) &&

               base.      Equals(UnitsOfMeasure);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
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

            => $"{Unit}{(Multiplier != 0 ? $"*10^{Multiplier}" : "")}";

        #endregion

    }

}
