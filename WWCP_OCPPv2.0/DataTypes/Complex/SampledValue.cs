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

using System;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A sampled (energy) meter value.
    /// </summary>
    public class SampledValue
    {

        #region Properties

        /// <summary>
        /// The measured value.
        /// </summary>
        public Decimal                Value               { get; }

        /// <summary>
        /// 
        /// </summary>
        public ReadingContexts?       Context             { get; }

        /// <summary>
        /// 
        /// </summary>
        public Measurands?            Measurand           { get; }

        /// <summary>
        /// 
        /// </summary>
        public Phases?                Phase               { get; }

        /// <summary>
        /// 
        /// </summary>
        public MeasurementLocations?  Location            { get; }

        /// <summary>
        /// 
        /// </summary>
        public SignedMeterValue       SignedMeterValue    { get; }

        /// <summary>
        /// 
        /// </summary>
        public UnitsOfMeasure         UnitOfMeasure       { get; }

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData             CustomData          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new sampled (energy) meter value.
        /// </summary>
        /// <param name="Value">The measured value.</param>
        /// <param name="Context"></param>
        /// <param name="Measurand"></param>
        /// <param name="Phase"></param>
        /// <param name="Location"></param>
        /// <param name="SignedMeterValue"></param>
        /// <param name="UnitOfMeasure"></param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public SampledValue(Decimal                Value,
                            ReadingContexts?       Context            = null,
                            Measurands?            Measurand          = null,
                            Phases?                Phase              = null,
                            MeasurementLocations?  Location           = null,
                            SignedMeterValue       SignedMeterValue   = null,
                            UnitsOfMeasure         UnitOfMeasure      = null,
                            CustomData             CustomData         = null)
        {

            this.Value             = Value;
            this.Context           = Context;
            this.Measurand         = Measurand;
            this.Phase             = Phase;
            this.Location          = Location;
            this.SignedMeterValue  = SignedMeterValue;
            this.UnitOfMeasure     = UnitOfMeasure;
            this.CustomData        = CustomData;

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:SampledValueType",
        //   "comment": "OCPP 2.0.1 FINAL",
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

        #region (static) Parse   (SampledValueJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="SampledValueJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SampledValue Parse(JObject              SampledValueJSON,
                                         OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(SampledValueJSON,
                         out SampledValue evse,
                         OnException))
            {
                return evse;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (SampledValueText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a communication module.
        /// </summary>
        /// <param name="SampledValueText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SampledValue Parse(String               SampledValueText,
                                         OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(SampledValueText,
                         out SampledValue evse,
                         OnException))
            {
                return evse;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out SampledValue, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a communication module.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="SampledValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out SampledValue     SampledValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                SampledValue = default;

                #region Value

                if (!JSON.ParseMandatory("value",
                                                     "value",
                                                     out Decimal  Value,
                                                     out String   ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Context

                if (JSON.ParseOptional("context",
                                                   "context",
                                                   ReadingContextsExtentions.Parse,
                                                   out ReadingContexts?  Context,
                                                   out                   ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Measurand

                if (JSON.ParseOptional("measurand",
                                                   "measurand",
                                                   MeasurandsExtentions.Parse,
                                                   out Measurands?  Measurand,
                                                   out              ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Phase

                if (JSON.ParseOptional("phase",
                                                   "phase",
                                                   PhasesExtentions.Parse,
                                                   out Phases?  Phase,
                                                   out          ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Location

                if (JSON.ParseOptional("location",
                                                   "measurment location",
                                                   MeasurementLocationsExtentions.Parse,
                                                   out MeasurementLocations?  Location,
                                                   out                        ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region SignedMeterValue

                if (JSON.ParseOptional("signedMeterValue",
                                                   "signed meter value",
                                                   OCPPv2_0.SignedMeterValue.TryParse,
                                                   out SignedMeterValue  SignedMeterValue,
                                                   out                   ErrorResponse,
                                                   OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region UnitOfMeasure

                if (JSON.ParseOptional("unitOfMeasure",
                                                   "unit of measure",
                                                   UnitsOfMeasure.TryParse,
                                                   out UnitsOfMeasure  UnitOfMeasure,
                                                   out                 ErrorResponse,
                                                   OnException))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region CustomData

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           OCPPv2_0.CustomData.TryParse,
                                           out CustomData  CustomData,
                                           out             ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, JSON, e);

                SampledValue = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(SampledValueText, out SampledValue, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a communication module.
        /// </summary>
        /// <param name="SampledValueText">The text to be parsed.</param>
        /// <param name="SampledValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               SampledValueText,
                                       out SampledValue     SampledValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                SampledValueText = SampledValueText?.Trim();

                if (SampledValueText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(SampledValueText),
                             out SampledValue,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, SampledValueText, e);
            }

            SampledValue = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomSampledValueResponseSerializer = null, ..., CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSampledValueResponseSerializer">A delegate to serialize custom SampledValue objects.</param>
        /// <param name="CustomSignedMeterValueResponseSerializer">A delegate to serialize custom SignedMeterValue objects.</param>
        /// <param name="CustomUnitsOfMeasureResponseSerializer">A delegate to serialize custom UnitsOfMeasure objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SampledValue>     CustomSampledValueResponseSerializer       = null,
                              CustomJObjectSerializerDelegate<SignedMeterValue> CustomSignedMeterValueResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<UnitsOfMeasure>   CustomUnitsOfMeasureResponseSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>       CustomCustomDataResponseSerializer         = null)
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

                           SignedMeterValue != null
                               ? new JProperty("signedMeterValue",  SignedMeterValue.ToJSON(CustomSignedMeterValueResponseSerializer,
                                                                                            CustomCustomDataResponseSerializer))
                               : null,

                           UnitOfMeasure != null
                               ? new JProperty("unitOfMeasure",     UnitOfMeasure.   ToJSON(CustomUnitsOfMeasureResponseSerializer,
                                                                                            CustomCustomDataResponseSerializer))
                               : null,

                           CustomData != null
                               ? new JProperty("customData",        CustomData.      ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomSampledValueResponseSerializer != null
                       ? CustomSampledValueResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (SampledValue1, SampledValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SampledValue1">An id tag info.</param>
        /// <param name="SampledValue2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SampledValue SampledValue1, SampledValue SampledValue2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(SampledValue1, SampledValue2))
                return true;

            // If one is null, but not both, return false.
            if (SampledValue1 is null || SampledValue2 is null)
                return false;

            if (SampledValue1 is null)
                throw new ArgumentNullException(nameof(SampledValue1),  "The given id tag info must not be null!");

            return SampledValue1.Equals(SampledValue2);

        }

        #endregion

        #region Operator != (SampledValue1, SampledValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SampledValue1">An id tag info.</param>
        /// <param name="SampledValue2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SampledValue SampledValue1, SampledValue SampledValue2)
            => !(SampledValue1 == SampledValue2);

        #endregion

        #endregion

        #region IEquatable<SampledValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is SampledValue SampledValue))
                return false;

            return Equals(SampledValue);

        }

        #endregion

        #region Equals(SampledValue)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="SampledValue">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SampledValue SampledValue)
        {

            if (SampledValue is null)
                return false;

            return Value.Equals(SampledValue.Value) &&

                   ((!Context.HasValue   && !SampledValue.Context.HasValue) ||
                     (Context.HasValue   &&  SampledValue.Context.HasValue   && Context.  Value.Equals(SampledValue.Context.  Value))) &&

                   ((!Measurand.HasValue && !SampledValue.Measurand.HasValue) ||
                     (Measurand.HasValue &&  SampledValue.Measurand.HasValue && Measurand.Value.Equals(SampledValue.Measurand.Value))) &&

                   ((!Phase.HasValue     && !SampledValue.Phase.HasValue) ||
                     (Phase.HasValue     &&  SampledValue.Phase.HasValue     && Phase.    Value.Equals(SampledValue.Phase.    Value))) &&

                   ((!Location.HasValue  && !SampledValue.Location.HasValue) ||
                     (Location.HasValue  &&  SampledValue.Location.HasValue  && Location. Value.Equals(SampledValue.Location. Value))) &&

                   ((SignedMeterValue == null && SampledValue.SignedMeterValue == null) ||
                    (SignedMeterValue != null && SampledValue.SignedMeterValue != null   && SignedMeterValue.Equals(SampledValue.SignedMeterValue))) &&

                   ((UnitOfMeasure    == null && SampledValue.UnitOfMeasure    == null) ||
                    (UnitOfMeasure    != null && SampledValue.UnitOfMeasure    != null   && UnitOfMeasure.   Equals(SampledValue.UnitOfMeasure)))    &&

                   ((CustomData       == null && SampledValue.CustomData       == null) ||
                    (CustomData       != null && SampledValue.CustomData       != null   && CustomData.      Equals(SampledValue.CustomData)));

        }

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

                return Value.                  GetHashCode() * 21 ^

                       (Context.HasValue
                            ? Context.         GetHashCode() * 17
                            : 0) ^

                       (Measurand.HasValue
                            ? Measurand.       GetHashCode() * 13
                            : 0) ^

                       (Phase.HasValue
                            ? Phase.           GetHashCode() * 11
                            : 0) ^

                       (Location.HasValue
                            ? Location.        GetHashCode() *  7
                            : 0) ^

                       (SignedMeterValue != null
                            ? SignedMeterValue.GetHashCode() *  5
                            : 0) ^

                       (UnitOfMeasure != null
                            ? UnitOfMeasure.   GetHashCode() *  3
                            : 0) ^

                       (CustomData != null
                            ? CustomData.      GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Value, UnitOfMeasure != null ? " " + UnitOfMeasure.Unit : "");

        #endregion

    }

}
