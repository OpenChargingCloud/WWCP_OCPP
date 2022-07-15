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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_0
{

    /// <summary>
    /// A (energy) meter value.
    /// </summary>
    public class MeterValue
    {

        #region Properties

        /// <summary>
        /// The sampled (energy) meter values.
        /// </summary>
        public IEnumerable<SampledValue>  SampledValues    { get; }

        /// <summary>
        /// The common timestamp of all sampled (energy) meter values.
        /// </summary>
        public DateTime                   Timestamp        { get; }

        /// <summary>
        /// An optional custom data object to allow to store any kind of customer specific data.
        /// </summary>
        public CustomData                 CustomData       { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new (energy) meter value.
        /// </summary>
        /// <param name="SampledValues">The sampled (energy) meter values.</param>
        /// <param name="Timestamp">The common timestamp of all sampled (energy) meter values.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public MeterValue(IEnumerable<SampledValue>  SampledValues,
                          DateTime                   Timestamp,
                          CustomData                 CustomData   = null)
        {

            this.SampledValues  = SampledValues;
            this.Timestamp      = Timestamp;
            this.CustomData     = CustomData;

            if (!this.SampledValues.SafeAny())
                throw new ArgumentNullException(nameof(SampledValues), "The given enumeration of sampled (energy) meter values must not be null or empty!");

        }

        #endregion


        #region Documentation

        // {
        //   "$schema": "http://json-schema.org/draft-06/schema#",
        //   "$id": "urn:OCPP:Cp:2:2020:3:MeterValueType",
        //   "comment": "OCPP 2.0.1 FINAL",
        //   "description": "Meter_ Value\r\nurn:x-oca:ocpp:uid:2:233265\r\nCollection of one or more sampled values in MeterValuesRequest and TransactionEvent. All sampled values in a MeterValue are sampled at the same point in time.\r\n",
        //   "javaType": "MeterValue",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "customData": {
        //       "$ref": "#/definitions/CustomDataType"
        //     },
        //     "sampledValue": {
        //       "type": "array",
        //       "additionalItems": false,
        //       "items": {
        //         "$ref": "#/definitions/SampledValueType"
        //       },
        //       "minItems": 1
        //     },
        //     "timestamp": {
        //       "description": "Meter_ Value. Timestamp. Date_ Time\r\nurn:x-oca:ocpp:uid:1:569259\r\nTimestamp for measured value(s).\r\n",
        //       "type": "string",
        //       "format": "date-time"
        //     }
        //   },
        //   "required": [
        //     "timestamp",
        //     "sampledValue"
        //   ]
        // }

        #endregion

        #region (static) Parse   (MeterValueJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of an (energy) meter value.
        /// </summary>
        /// <param name="MeterValueJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValue Parse(JObject              MeterValueJSON,
                                       OnExceptionDelegate  OnException   = null)
        {

            if (TryParse(MeterValueJSON,
                         out MeterValue evse,
                         OnException))
            {
                return evse;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (MeterValueText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an (energy) meter value.
        /// </summary>
        /// <param name="MeterValueText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValue Parse(String               MeterValueText,
                                       OnExceptionDelegate  OnException   = null)
        {


            if (TryParse(MeterValueText,
                         out MeterValue evse,
                         OnException))
            {
                return evse;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(JSON, out MeterValue, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of an (energy) meter value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MeterValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out MeterValue       MeterValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                MeterValue = default;

                #region SampledValues

                if (!JSON.ParseMandatoryJSON("sampledValue",
                                                       "sampled (energy) meter values",
                                                       SampledValue.TryParse,
                                                       out IEnumerable<SampledValue>  SampledValues,
                                                       out String                     ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp

                if (!JSON.ParseMandatory("timestamp",
                                                   "common timestamp",
                                                   out DateTime  Timestamp,
                                                   out           ErrorResponse))
                {
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


                MeterValue = new MeterValue(SampledValues,
                                            Timestamp,
                                            CustomData);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, JSON, e);

                MeterValue = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(MeterValueText, out MeterValue, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an (energy) meter value.
        /// </summary>
        /// <param name="MeterValueText">The text to be parsed.</param>
        /// <param name="MeterValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               MeterValueText,
                                       out MeterValue       MeterValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                MeterValueText = MeterValueText?.Trim();

                if (MeterValueText.IsNotNullOrEmpty() &&
                    TryParse(JObject.Parse(MeterValueText),
                             out MeterValue,
                             OnException))
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, MeterValueText, e);
            }

            MeterValue = default;
            return false;

        }

        #endregion

        #region ToJSON(CustomMeterValueResponseSerializer = null, ..., CustomCustomDataResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterValueResponseSerializer">A delegate to serialize custom MeterValue objects.</param>
        /// <param name="CustomSampledValueResponseSerializer">A delegate to serialize custom SampledValue objects.</param>
        /// <param name="CustomSignedMeterValueResponseSerializer">A delegate to serialize custom SignedMeterValue objects.</param>
        /// <param name="CustomUnitsOfMeasureResponseSerializer">A delegate to serialize custom UnitsOfMeasure objects.</param>
        /// <param name="CustomCustomDataResponseSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterValue>       CustomMeterValueResponseSerializer         = null,
                              CustomJObjectSerializerDelegate<SampledValue>     CustomSampledValueResponseSerializer       = null,
                              CustomJObjectSerializerDelegate<SignedMeterValue> CustomSignedMeterValueResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<UnitsOfMeasure>   CustomUnitsOfMeasureResponseSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>       CustomCustomDataResponseSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("sampledValue",      new JArray(SampledValues.SafeSelect(sampledValue => sampledValue.ToJSON(CustomSampledValueResponseSerializer,
                                                                                                                                      CustomSignedMeterValueResponseSerializer,
                                                                                                                                      CustomUnitsOfMeasureResponseSerializer,
                                                                                                                                      CustomCustomDataResponseSerializer)))),
                           new JProperty("timestamp",         Timestamp.ToIso8601()),

                           CustomData != null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomMeterValueResponseSerializer != null
                       ? CustomMeterValueResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (MeterValue1, MeterValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterValue1">An id tag info.</param>
        /// <param name="MeterValue2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MeterValue MeterValue1, MeterValue MeterValue2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MeterValue1, MeterValue2))
                return true;

            // If one is null, but not both, return false.
            if (MeterValue1 is null || MeterValue2 is null)
                return false;

            if (MeterValue1 is null)
                throw new ArgumentNullException(nameof(MeterValue1),  "The given id tag info must not be null!");

            return MeterValue1.Equals(MeterValue2);

        }

        #endregion

        #region Operator != (MeterValue1, MeterValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterValue1">An id tag info.</param>
        /// <param name="MeterValue2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MeterValue MeterValue1, MeterValue MeterValue2)
            => !(MeterValue1 == MeterValue2);

        #endregion

        #endregion

        #region IEquatable<MeterValue> Members

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

            if (!(Object is MeterValue MeterValue))
                return false;

            return Equals(MeterValue);

        }

        #endregion

        #region Equals(MeterValue)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="MeterValue">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(MeterValue MeterValue)
        {

            if (MeterValue is null)
                return false;

            return //ToDo: Compare SampledValues!
                   Timestamp.Equals(MeterValue.Timestamp) &&

                   ((CustomData == null && MeterValue.CustomData == null) ||
                    (CustomData != null && MeterValue.CustomData != null && CustomData.Equals(MeterValue.CustomData)));

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

                return //ToDo: Add SampledValues!
                       Timestamp.GetHashCode() * 3 ^

                       (CustomData != null
                            ? CustomData.GetHashCode()
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Timestamp, ": ", SampledValues.AggregateWith(", "));

        #endregion

    }

}
