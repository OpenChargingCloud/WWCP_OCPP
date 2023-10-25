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

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A metering value.
    /// </summary>
    public class MeteringValue : ACustomData,
                                 IEquatable<MeteringValue>
    {

        #region Properties

        /// <summary>
        /// The timestamp of the value.
        /// </summary>
        public DateTime               Timestamp           { get; }

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
        /// The optional indication where the measured value has been metering.
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
        /// Create a new metering value.
        /// </summary>
        /// <param name="Value">A measured value.</param>
        /// <param name="Context">An optional type of the value.</param>
        /// <param name="Measurand">An optional type of the measurement. Default: "Energy.Active.Import.Register".</param>
        /// <param name="Phase">An optional indication how to interpret the measured value.</param>
        /// <param name="Location">An optional indication where the measured value has been metering. Default: "Outlet".</param>
        /// <param name="SignedMeterValue">An optional meter value with signature and encoding information.</param>
        /// <param name="UnitOfMeasure">An optional unit of measure including a multiplier.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public MeteringValue(DateTime               Timestamp,
                             Decimal                Value,
                             ReadingContexts?       Context            = null,
                             Measurands?            Measurand          = null,
                             Phases?                Phase              = null,
                             MeasurementLocations?  Location           = null,
                             SignedMeterValue?      SignedMeterValue   = null,
                             UnitsOfMeasure?        UnitOfMeasure      = null,
                             CustomData?            CustomData         = null)

            : base(CustomData)

        {

            this.Timestamp         = Timestamp;
            this.Value             = Value;
            this.Context           = Context;
            this.Measurand         = Measurand;
            this.Phase             = Phase;
            this.Location          = Location;
            this.SignedMeterValue  = SignedMeterValue;
            this.UnitOfMeasure     = UnitOfMeasure;

            unchecked
            {

                hashCode = Value.            GetHashCode()       * 19 ^

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


        #region Documentation

        // tba.

        #endregion

        #region (static) Parse   (JSON, CustomMeteringValueParser = null)

        /// <summary>
        /// Parse the given JSON representation of a metering value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomMeteringValueParser">A delegate to parse custom metering values.</param>
        public static MeteringValue Parse(JObject                                      JSON,
                                          CustomJObjectParserDelegate<MeteringValue>?  CustomMeteringValueParser   = null)
        {

            if (TryParse(JSON,
                         out var meteringValue,
                         out var errorResponse,
                         CustomMeteringValueParser))
            {
                return meteringValue!;
            }

            throw new ArgumentException("The given JSON representation of a metering value is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out MeteringValue, out ErrorResponse, CustomMeteringValueParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a metering value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MeteringValue">The parsed metering value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject             JSON,
                                       out MeteringValue?  MeteringValue,
                                       out String?         ErrorResponse)

            => TryParse(JSON,
                        out MeteringValue,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a metering value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MeteringValue">The parsed metering value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomMeteringValueParser">A delegate to parse custom metering values.</param>
        public static Boolean TryParse(JObject                                      JSON,
                                       out MeteringValue?                           MeteringValue,
                                       out String?                                  ErrorResponse,
                                       CustomJObjectParserDelegate<MeteringValue>?  CustomMeteringValueParser   = null)
        {

            try
            {

                MeteringValue = default;

                #region Timestamp           [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "metering timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Value               [mandatory]

                if (!JSON.ParseMandatory("value",
                                         "metering value",
                                         out Decimal Value,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Context             [optional]

                if (JSON.ParseOptional("context",
                                       "context",
                                       ReadingContextsExtensions.TryParse,
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
                                       MeasurandsExtensions.TryParse,
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
                                       PhasesExtensions.TryParse,
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
                                       MeasurementLocationsExtensions.TryParse,
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
                                           OCPPv2_1.SignedMeterValue.TryParse,
                                           out SignedMeterValue SignedMeterValue,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region UnitOfMeasure       [optional]

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

                #region CustomData          [optional]

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_1.CustomData.TryParse,
                                           out CustomData CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                MeteringValue = new MeteringValue(
                                    Timestamp,
                                    Value,
                                    Context,
                                    Measurand,
                                    Phase,
                                    Location,
                                    SignedMeterValue,
                                    UnitOfMeasure,
                                    CustomData
                                );

                if (CustomMeteringValueParser is not null)
                    MeteringValue = CustomMeteringValueParser(JSON,
                                                              MeteringValue);

                return true;

            }
            catch (Exception e)
            {
                MeteringValue  = default;
                ErrorResponse  = "The given JSON representation of a metering value is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomMeteringValueSerializer = null, ..., CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeteringValueSerializer">A delegate to serialize custom metering values.</param>
        /// <param name="CustomSignedMeterValueSerializer">A delegate to serialize custom signed meter values.</param>
        /// <param name="CustomUnitsOfMeasureSerializer">A delegate to serialize custom units of measure.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeteringValue>?    CustomMeteringValueSerializer      = null,
                              CustomJObjectSerializerDelegate<SignedMeterValue>? CustomSignedMeterValueSerializer   = null,
                              CustomJObjectSerializerDelegate<UnitsOfMeasure>?   CustomUnitsOfMeasureSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("timestamp",          Timestamp.       ToIso8601()),
                                 new JProperty("value",              Value),

                           Context.HasValue
                               ? new JProperty("context",            Context.   Value.AsText())
                               : null,

                           Measurand.HasValue
                               ? new JProperty("measurand",          Measurand. Value.AsText())
                               : null,

                           Phase.HasValue
                               ? new JProperty("phase",              Phase.     Value.AsText())
                               : null,

                           Location.HasValue
                               ? new JProperty("location",           Location.  Value.AsText())
                               : null,

                           SignedMeterValue is not null
                               ? new JProperty("signedMeterValue",   SignedMeterValue.ToJSON(CustomSignedMeterValueSerializer,
                                                                                             CustomCustomDataSerializer))
                               : null,

                           UnitOfMeasure is not null
                               ? new JProperty("unitOfMeasure",      UnitOfMeasure.   ToJSON(CustomUnitsOfMeasureSerializer,
                                                                                             CustomCustomDataSerializer))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomMeteringValueSerializer is not null
                       ? CustomMeteringValueSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this metering value.
        /// </summary>
        public MeteringValue Clone()

            => new (
                   Timestamp,
                   Value,
                   Context,
                   Measurand,
                   Phase,
                   Location,
                   SignedMeterValue?.Clone(),
                   UnitOfMeasure?.   Clone(),
                   CustomData
               );

        #endregion


        #region Operator overloading

        #region Operator == (MeteringValue1, MeteringValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeteringValue1">A metering value.</param>
        /// <param name="MeteringValue2">Another metering value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MeteringValue? MeteringValue1,
                                           MeteringValue? MeteringValue2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MeteringValue1, MeteringValue2))
                return true;

            // If one is null, but not both, return false.
            if (MeteringValue1 is null || MeteringValue2 is null)
                return false;

            return MeteringValue1.Equals(MeteringValue2);

        }

        #endregion

        #region Operator != (MeteringValue1, MeteringValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeteringValue1">A metering value.</param>
        /// <param name="MeteringValue2">Another metering value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MeteringValue? MeteringValue1,
                                           MeteringValue? MeteringValue2)

            => !(MeteringValue1 == MeteringValue2);

        #endregion

        #endregion

        #region IEquatable<MeteringValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two metering values for equality.
        /// </summary>
        /// <param name="Object">A metering value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MeteringValue meteringValue &&
                   Equals(meteringValue);

        #endregion

        #region Equals(MeteringValue)

        /// <summary>
        /// Compares two metering values for equality.
        /// </summary>
        /// <param name="MeteringValue">A metering value to compare with.</param>
        public Boolean Equals(MeteringValue? MeteringValue)

            => MeteringValue is not null &&

               Value.                Equals(MeteringValue.Value)                 &&
               Timestamp.ToIso8601().Equals(MeteringValue.Timestamp.ToIso8601()) &&

            ((!Context.HasValue   && !MeteringValue.Context.HasValue)   ||
              (Context.HasValue   &&  MeteringValue.Context.HasValue   && Context.  Value.Equals(MeteringValue.Context.  Value))) &&

            ((!Measurand.HasValue && !MeteringValue.Measurand.HasValue) ||
              (Measurand.HasValue &&  MeteringValue.Measurand.HasValue && Measurand.Value.Equals(MeteringValue.Measurand.Value))) &&

            ((!Phase.HasValue     && !MeteringValue.Phase.HasValue)     ||
              (Phase.HasValue     &&  MeteringValue.Phase.HasValue     && Phase.    Value.Equals(MeteringValue.Phase.    Value))) &&

            ((!Location.HasValue  && !MeteringValue.Location.HasValue)  ||
              (Location.HasValue  &&  MeteringValue.Location.HasValue  && Location. Value.Equals(MeteringValue.Location. Value))) &&

            ((SignedMeterValue is     null && MeteringValue.SignedMeterValue is     null) ||
             (SignedMeterValue is not null && MeteringValue.SignedMeterValue is not null   && SignedMeterValue.Equals(MeteringValue.SignedMeterValue))) &&

            ((UnitOfMeasure    is     null && MeteringValue.UnitOfMeasure    is     null) ||
             (UnitOfMeasure    is not null && MeteringValue.UnitOfMeasure    is not null   && UnitOfMeasure.   Equals(MeteringValue.UnitOfMeasure)))    &&

              base.Equals(MeteringValue);

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

            => $"{Timestamp}: {Value}{(UnitOfMeasure is not null ? $" {UnitOfMeasure.Unit}" : "")}";

        #endregion

    }

}
