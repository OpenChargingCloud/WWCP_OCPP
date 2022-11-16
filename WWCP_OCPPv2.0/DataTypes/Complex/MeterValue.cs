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
    /// A meter value.
    /// </summary>
    public class MeterValue : ACustomData
    {

        #region Properties

        /// <summary>
        /// The sampled meter values.
        /// </summary>
        public IEnumerable<SampledValue>  SampledValues    { get; }

        /// <summary>
        /// The common timestamp of all sampled meter values.
        /// </summary>
        public DateTime                   Timestamp        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter value.
        /// </summary>
        /// <param name="SampledValues">The sampled meter values.</param>
        /// <param name="Timestamp">The common timestamp of all sampled meter values.</param>
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public MeterValue(IEnumerable<SampledValue>  SampledValues,
                          DateTime                   Timestamp,
                          CustomData?                CustomData   = null)

            : base(CustomData)

        {

            this.SampledValues  = SampledValues;
            this.Timestamp      = Timestamp;

            if (!this.SampledValues.SafeAny())
                throw new ArgumentNullException(nameof(SampledValues), "The given enumeration of sampled meter values must not be null or empty!");

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

        #region (static) Parse   (JSON, CustomMeterValueParser = null)

        /// <summary>
        /// Parse the given JSON representation of a meter value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomMeterValueParser">A delegate to parse custom meter values.</param>
        public static MeterValue Parse(JObject                                   JSON,
                                       CustomJObjectParserDelegate<MeterValue>?  CustomMeterValueParser   = null)
        {

            if (TryParse(JSON,
                         out var meterValue,
                         out var errorResponse,
                         CustomMeterValueParser))
            {
                return meterValue!;
            }

            throw new ArgumentException("The given JSON representation of a meter value is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out MeterValue, out ErrorResponse, CustomMeterValueParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a meter value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MeterValue">The parsed meter value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject          JSON,
                                       out MeterValue?  MeterValue,
                                       out String?      ErrorResponse)

            => TryParse(JSON,
                        out MeterValue,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a meter value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="MeterValue">The parsed meter value.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomMeterValueParser">A delegate to parse custom meter values.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       out MeterValue?                           MeterValue,
                                       out String?                               ErrorResponse,
                                       CustomJObjectParserDelegate<MeterValue>?  CustomMeterValueParser)
        {

            try
            {

                MeterValue = default;

                #region SampledValues    [mandatory]

                if (!JSON.ParseMandatoryJSON("sampledValue",
                                             "sampled meter values",
                                             SampledValue.TryParse,
                                             out IEnumerable<SampledValue> SampledValues,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Timestamp        [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "common timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region CustomData       [optional]

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


                MeterValue = new MeterValue(SampledValues,
                                            Timestamp,
                                            CustomData);

                if (CustomMeterValueParser is not null)
                    MeterValue = CustomMeterValueParser(JSON,
                                                        MeterValue);

                return true;

            }
            catch (Exception e)
            {
                MeterValue     = default;
                ErrorResponse  = "The given JSON representation of a meter value is invalid: " + e.Message;
                return false;
            }

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
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterValue>?       CustomMeterValueResponseSerializer         = null,
                              CustomJObjectSerializerDelegate<SampledValue>?     CustomSampledValueResponseSerializer       = null,
                              CustomJObjectSerializerDelegate<SignedMeterValue>? CustomSignedMeterValueResponseSerializer   = null,
                              CustomJObjectSerializerDelegate<UnitsOfMeasure>?   CustomUnitsOfMeasureResponseSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataResponseSerializer         = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("sampledValue",      new JArray(SampledValues.SafeSelect(sampledValue => sampledValue.ToJSON(CustomSampledValueResponseSerializer,
                                                                                                                                      CustomSignedMeterValueResponseSerializer,
                                                                                                                                      CustomUnitsOfMeasureResponseSerializer,
                                                                                                                                      CustomCustomDataResponseSerializer)))),
                           new JProperty("timestamp",         Timestamp.ToIso8601()),

                           CustomData is not null
                               ? new JProperty("customData",  CustomData.ToJSON(CustomCustomDataResponseSerializer))
                               : null

                       );

            return CustomMeterValueResponseSerializer is not null
                       ? CustomMeterValueResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (MeterValue1, MeterValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterValue1">A meter value.</param>
        /// <param name="MeterValue2">Another meter value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MeterValue MeterValue1,
                                           MeterValue MeterValue2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(MeterValue1, MeterValue2))
                return true;

            // If one is null, but not both, return false.
            if (MeterValue1 is null || MeterValue2 is null)
                return false;

            return MeterValue1.Equals(MeterValue2);

        }

        #endregion

        #region Operator != (MeterValue1, MeterValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterValue1">A meter value.</param>
        /// <param name="MeterValue2">Another meter value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (MeterValue MeterValue1,
                                           MeterValue MeterValue2)

            => !(MeterValue1 == MeterValue2);

        #endregion

        #endregion

        #region IEquatable<MeterValue> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two meter values for equality.
        /// </summary>
        /// <param name="Object">A meter value to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is MeterValue meterValue &&
                   Equals(meterValue);

        #endregion

        #region Equals(MeterValue)

        /// <summary>
        /// Compares two meter values for equality.
        /// </summary>
        /// <param name="MeterValue">A meter value to compare with.</param>
        public Boolean Equals(MeterValue MeterValue)

            => MeterValue is not null &&

               //ToDo: Compare SampledValues!
               Timestamp.Equals(MeterValue.Timestamp) &&

               base.     Equals(MeterValue);

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

                return Timestamp.GetHashCode() * 3 ^

                       //ToDo: Add SampledValues!

                       base.     GetHashCode();

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
