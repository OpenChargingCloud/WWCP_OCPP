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
using System.Numerics;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A sampled (energy) meter value.
    /// </summary>
    public class SampledValue : ACustomData,
                                IEquatable<SampledValue>
    {

        #region Properties

        /// <summary>
        /// The measured value.
        /// </summary>
        [Mandatory]
        public Decimal                Value               { get; }

        /// <summary>
        /// The optional type of the value.
        /// </summary>
        [Optional]
        public ReadingContexts?       Context             { get; }

        /// <summary>
        /// The optional type of the measurement.
        /// Default: "Energy.Active.Import.Register".
        /// </summary>
        [Optional]
        public Measurands?            Measurand           { get; }

        /// <summary>
        /// The optional indication how to interpret the measured value.
        /// Please note that not all values of phase are applicable to all Measurands.
        /// When phase is absent, the measured value is interpreted as an overall value.
        /// </summary>
        [Optional]
        public Phases?                Phase               { get; }

        /// <summary>
        /// The optional indication where the measured value has been sampled.
        /// Default: "Outlet".
        /// </summary>
        [Optional]
        public MeasurementLocations?  Location            { get; }

        /// <summary>
        /// The optional meter value with signature and encoding information.
        /// </summary>
        [Optional]
        public SignedMeterValue?      SignedMeterValue    { get; }

        /// <summary>
        /// The optional unit of measure including a multiplier.
        /// </summary>
        [Optional]
        public UnitsOfMeasure?        UnitOfMeasure       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new sampled (energy) meter value.
        /// </summary>
        /// <param name="Value">A measured value.</param>
        /// <param name="Context">An optional type of the value.</param>
        /// <param name="Measurand">An optional type of the measurement. Default: "Energy.Active.Import.Register".</param>
        /// <param name="Phase">An optional indication how to interpret the measured value.</param>
        /// <param name="Location">An optional indication where the measured value has been sampled. Default: "Outlet".</param>
        /// <param name="SignedMeterValue">An optional meter value with signature and encoding information.</param>
        /// <param name="UnitOfMeasure">An optional unit of measure including a multiplier.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SampledValue(Decimal                Value,
                            ReadingContexts?       Context            = null,
                            Measurands?            Measurand          = null,
                            Phases?                Phase              = null,
                            MeasurementLocations?  Location           = null,
                            SignedMeterValue?      SignedMeterValue   = null,
                            UnitsOfMeasure?        UnitOfMeasure      = null,
                            CustomData?            CustomData         = null)

            : base(CustomData)

        {

            this.Value             = Value;
            this.Context           = Context;
            this.Measurand         = Measurand;
            this.Phase             = Phase;
            this.Location          = Location;
            this.SignedMeterValue  = SignedMeterValue;
            this.UnitOfMeasure     = UnitOfMeasure;

        }

        #endregion


        #region Documentation

        // "SampledValueType": {
        //   "description": "Sampled_ Value\r\nurn:x-oca:ocpp:uid:2:233266\r\nSingle sampled value in MeterValues. Each value can be accompanied by optional fields.\r\n\r\nTo save on mobile data usage, default values of all of the optional fields are such that. The value without any additional fields will be interpreted, as a register reading of active import energy in Wh (Watt-hour) units.\r\n",
        //   "javaType": "SampledValue",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "value": {
        //       "description": "Sampled_ Value. Value. Measure\r\nurn:x-oca:ocpp:uid:1:569260\r\nIndicates the measured value.\r\n\r\n",
        //       "type": "number"
        //     },
        //     "context": {
        //       "$ref": "#/definitions/ReadingContextEnumType"
        //     },
        //     "measurand": {
        //       "$ref": "#/definitions/MeasurandEnumType"
        //     },
        //     "phase": {
        //       "$ref": "#/definitions/PhaseEnumType"
        //     },
        //     "location": {
        //       "$ref": "#/definitions/LocationEnumType"
        //     },
        //     "signedMeterValue": {
        //       "$ref": "#/definitions/SignedMeterValueType"
        //     },
        //     "unitOfMeasure": {
        //       "$ref": "#/definitions/UnitOfMeasureType"
        //     }
        //   },
        //   "required": [
        //     "value"
        //   ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomSampledValueParser = null)

        /// <summary>
        /// Parse the given JSON representation of a sampled value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomSampledValueParser">A delegate to parse custom sampled values.</param>
        public static SampledValue Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<SampledValue>?  CustomSampledValueParser   = null)
        {

            if (TryParse(JSON,
                         out var sampledValue,
                         out var errorResponse,
                         CustomSampledValueParser))
            {
                return sampledValue!;
            }

            throw new ArgumentException("The given JSON representation of a sampled value is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out SampledValue, out ErrorResponse, CustomSampledValueParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a sampled value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SampledValue">The parsed sampled value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject            JSON,
                                       out SampledValue?  SampledValue,
                                       out String?        ErrorResponse)

            => TryParse(JSON,
                        out SampledValue,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a sampled value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SampledValue">The parsed sampled value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSampledValueParser">A delegate to parse custom sampled values.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       out SampledValue?                           SampledValue,
                                       out String?                                 ErrorResponse,
                                       CustomJObjectParserDelegate<SampledValue>?  CustomSampledValueParser   = null)
        {

            try
            {

                SampledValue = default;

                #region Value               [mandatory]

                if (!JSON.ParseMandatory("value",
                                         "value",
                                         out Decimal Value,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Context             [optional]

                if (JSON.ParseOptional("context",
                                       "context",
                                       ReadingContextsExtentions.Parse,
                                       out ReadingContexts? Context,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Measurand           [optional]

                if (JSON.ParseOptional("measurand",
                                       "measurand",
                                       MeasurandsExtentions.Parse,
                                       out Measurands? Measurand,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Phase               [optional]

                if (JSON.ParseOptional("phase",
                                       "phase",
                                       PhasesExtentions.Parse,
                                       out Phases? Phase,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Location            [optional]

                if (JSON.ParseOptional("location",
                                       "measurment location",
                                       MeasurementLocationsExtentions.Parse,
                                       out MeasurementLocations? Location,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region SignedMeterValue    [optional]

                if (JSON.ParseOptionalJSON("signedMeterValue",
                                           "signed meter value",
                                           OCPPv2_0.SignedMeterValue.TryParse,
                                           out SignedMeterValue SignedMeterValue,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region UnitOfMeasure

                if (JSON.ParseOptionalJSON("unitOfMeasure",
                                           "unit of measure",
                                           UnitsOfMeasure.TryParse,
                                           out UnitsOfMeasure UnitOfMeasure,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region CustomData

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


                SampledValue = new SampledValue(Value,
                                                Context,
                                                Measurand,
                                                Phase,
                                                Location,
                                                SignedMeterValue,
                                                UnitOfMeasure,
                                                CustomData);

                if (CustomSampledValueParser is not null)
                    SampledValue = CustomSampledValueParser(JSON,
                                                            SampledValue);

                return true;

            }
            catch (Exception e)
            {
                SampledValue   = default;
                ErrorResponse  = "The given JSON representation of a sampled value is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomSampledValueSerializer = null, ..., CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        /// <param name="CustomSignedMeterValueSerializer">A delegate to serialize custom signed meter values.</param>
        /// <param name="CustomUnitsOfMeasureSerializer">A delegate to serialize custom units of measure.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SampledValue>?     CustomSampledValueSerializer       = null,
                              CustomJObjectSerializerDelegate<SignedMeterValue>? CustomSignedMeterValueSerializer   = null,
                              CustomJObjectSerializerDelegate<UnitsOfMeasure>?   CustomUnitsOfMeasureSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("value",                   Value),

                           Context.HasValue
                               ? new JProperty("context",           Context.   Value.AsText())
                               : null,

                           Measurand.HasValue
                               ? new JProperty("measurand",         Measurand. Value.AsText())
                               : null,

                           Phase.HasValue
                               ? new JProperty("phase",             Phase.     Value.AsText())
                               : null,

                           Location.HasValue
                               ? new JProperty("location",          Location.  Value.AsText())
                               : null,

                           SignedMeterValue is not null
                               ? new JProperty("signedMeterValue",  SignedMeterValue.ToJSON(CustomSignedMeterValueSerializer,
                                                                                            CustomCustomDataSerializer))
                               : null,

                           UnitOfMeasure is not null
                               ? new JProperty("unitOfMeasure",     UnitOfMeasure.   ToJSON(CustomUnitsOfMeasureSerializer,
                                                                                            CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",        CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomSampledValueSerializer is not null
                       ? CustomSampledValueSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SampledValue1, SampledValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SampledValue1">A sampled (energy) meter value.</param>
        /// <param name="SampledValue2">Another sampled (energy) meter value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SampledValue? SampledValue1,
                                           SampledValue? SampledValue2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SampledValue1, SampledValue2))
                return true;

            // If one is null, but not both, return false.
            if (SampledValue1 is null || SampledValue2 is null)
                return false;

            return SampledValue1.Equals(SampledValue2);

        }

        #endregion

        #region Operator != (SampledValue1, SampledValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SampledValue1">A sampled (energy) meter value.</param>
        /// <param name="SampledValue2">Another sampled (energy) meter value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SampledValue? SampledValue1,
                                           SampledValue? SampledValue2)

            => !(SampledValue1 == SampledValue2);

        #endregion

        #endregion

        #region IEquatable<SampledValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two sampled values for equality.
        /// </summary>
        /// <param name="Object">A sampled value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is SampledValue sampledValue &&
                   Equals(sampledValue);

        #endregion

        #region Equals(SampledValue)

        /// <summary>
        /// Compares two sampled values for equality.
        /// </summary>
        /// <param name="SampledValue">A sampled value to compare with.</param>
        public Boolean Equals(SampledValue? SampledValue)

            => SampledValue is not null &&

               Value.Equals(SampledValue.Value) &&

            ((!Context.HasValue   && !SampledValue.Context.HasValue) ||
              (Context.HasValue   &&  SampledValue.Context.HasValue   && Context.  Value.Equals(SampledValue.Context.  Value))) &&

            ((!Measurand.HasValue && !SampledValue.Measurand.HasValue) ||
              (Measurand.HasValue &&  SampledValue.Measurand.HasValue && Measurand.Value.Equals(SampledValue.Measurand.Value))) &&

            ((!Phase.HasValue     && !SampledValue.Phase.HasValue) ||
              (Phase.HasValue     &&  SampledValue.Phase.HasValue     && Phase.    Value.Equals(SampledValue.Phase.    Value))) &&

            ((!Location.HasValue  && !SampledValue.Location.HasValue) ||
              (Location.HasValue  &&  SampledValue.Location.HasValue  && Location. Value.Equals(SampledValue.Location. Value))) &&

            ((SignedMeterValue is     null && SampledValue.SignedMeterValue is     null) ||
             (SignedMeterValue is not null && SampledValue.SignedMeterValue is not null   && SignedMeterValue.Equals(SampledValue.SignedMeterValue))) &&

            ((UnitOfMeasure    is     null && SampledValue.UnitOfMeasure    is     null) ||
             (UnitOfMeasure    is not null && SampledValue.UnitOfMeasure    is not null   && UnitOfMeasure.   Equals(SampledValue.UnitOfMeasure)))    &&

              base.  Equals(SampledValue);

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

                return Value.            GetHashCode()       * 19 ^

                      (Context?.         GetHashCode() ?? 0) * 17 ^
                      (Measurand?.       GetHashCode() ?? 0) * 13 ^
                      (Phase?.           GetHashCode() ?? 0) * 11 ^
                      (Location?.        GetHashCode() ?? 0) *  7 ^
                      (SignedMeterValue?.GetHashCode() ?? 0) *  5 ^
                      (UnitOfMeasure?.   GetHashCode() ?? 0) *  3 ^

                      base.              GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Value,
                             UnitOfMeasure is not null
                                 ? " " + UnitOfMeasure.Unit
                                 : "");

        #endregion

    }

}
