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
    /// A meter value.
    /// </summary>
    public class MeterValue : ACustomData,
                              IEquatable<MeterValue>
    {

        #region Properties

        /// <summary>
        /// The common timestamp of all sampled meter values.
        /// </summary>
        public DateTime                   Timestamp        { get; }

        /// <summary>
        /// The sampled meter values.
        /// </summary>
        public IEnumerable<SampledValue>  SampledValues    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter value.
        /// </summary>
        /// <param name="Timestamp">The common timestamp of all sampled meter values.</param>
        /// <param name="SampledValues">The sampled meter values.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public MeterValue(DateTime                   Timestamp,
                          IEnumerable<SampledValue>  SampledValues,
                          CustomData?                CustomData   = null)

            : base(CustomData)

        {

            if (!SampledValues.Any())
                throw new ArgumentException("The given enumeration of sampled meter values must not be empty!",
                                            nameof(SampledValues));

            this.Timestamp      = Timestamp;
            this.SampledValues  = SampledValues.Distinct();

            unchecked
            {

                hashCode = Timestamp.    GetHashCode()  * 5 ^
                           SampledValues.CalcHashCode() * 3 ^

                           base.         GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Collection of one or more sampled values in MeterValuesRequest and TransactionEvent.
        //                     All sampled values in a MeterValue are sampled at the same point in time.",
        //     "javaType": "MeterValue",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "sampledValue": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/SampledValueType"
        //             },
        //             "minItems": 1
        //         },
        //         "timestamp": {
        //             "description": "Timestamp for measured value(s).",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "timestamp",
        //         "sampledValue"
        //     ]
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
                return meterValue;
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
        public static Boolean TryParse(JObject                               JSON,
                                       [NotNullWhen(true)]  out MeterValue?  MeterValue,
                                       [NotNullWhen(false)] out String?      ErrorResponse)

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
                                       [NotNullWhen(true)]  out MeterValue?      MeterValue,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
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
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion


                MeterValue = new MeterValue(
                                 Timestamp,
                                 SampledValues,
                                 CustomData
                             );

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

        #region ToJSON(CustomMeterValueSerializer = null, CustomSampledValueSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterValueSerializer">A delegate to serialize custom meter values.</param>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        /// <param name="CustomSignedMeterValueSerializer">A delegate to serialize custom signed meter values.</param>
        /// <param name="CustomUnitsOfMeasureSerializer">A delegate to serialize custom units of measure.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterValue>?       CustomMeterValueSerializer         = null,
                              CustomJObjectSerializerDelegate<SampledValue>?     CustomSampledValueSerializer       = null,
                              CustomJObjectSerializerDelegate<SignedMeterValue>? CustomSignedMeterValueSerializer   = null,
                              CustomJObjectSerializerDelegate<UnitsOfMeasure>?   CustomUnitsOfMeasureSerializer     = null,
                              CustomJObjectSerializerDelegate<CustomData>?       CustomCustomDataSerializer         = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("timestamp",      Timestamp.ToIso8601()),

                                 new JProperty("sampledValue",   new JArray(SampledValues.SafeSelect(sampledValue => sampledValue.ToJSON(CustomSampledValueSerializer,
                                                                                                                                         CustomSignedMeterValueSerializer,
                                                                                                                                         CustomUnitsOfMeasureSerializer,
                                                                                                                                         CustomCustomDataSerializer)))),

                           CustomData is not null
                               ? new JProperty("customData",     CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomMeterValueSerializer is not null
                       ? CustomMeterValueSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this meter value.
        /// </summary>
        public MeterValue Clone()

            => new (
                   Timestamp,
                   SampledValues.Select(sampledValue => sampledValue.Clone()).ToArray(),
                   CustomData
               );

        #endregion


        #region Operator overloading

        #region Operator == (MeterValue1, MeterValue2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MeterValue1">A meter value.</param>
        /// <param name="MeterValue2">Another meter value.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (MeterValue? MeterValue1,
                                           MeterValue? MeterValue2)
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
        public static Boolean operator != (MeterValue? MeterValue1,
                                           MeterValue? MeterValue2)

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
        public Boolean Equals(MeterValue? MeterValue)

            => MeterValue is not null &&

               Timestamp.Equals(MeterValue.Timestamp) &&

               SampledValues.Count().Equals(MeterValue.SampledValues.Count()) &&
               SampledValues.All(entry => MeterValue.SampledValues.Contains(entry)) &&

               base.     Equals(MeterValue);

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

            => $"{Timestamp}: {SampledValues.AggregateWith(", ")}";

        #endregion

    }

}
