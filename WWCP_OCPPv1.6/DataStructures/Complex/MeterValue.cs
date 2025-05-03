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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A meter value.
    /// </summary>
    public class MeterValue : IEquatable<MeterValue>
    {

        #region Properties

        /// <summary>
        /// The timestamp of the sampled value(s).
        /// </summary>
        public DateTime                   Timestamp       { get; }

        /// <summary>
        /// An enumeration of sampled values.
        /// </summary>
        public IEnumerable<SampledValue>  SampledValues   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new meter value.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the sampled value(s).</param>
        /// <param name="SampledValues">An enumeration of sampled values.</param>
        public MeterValue(DateTime                   Timestamp,
                          IEnumerable<SampledValue>  SampledValues)
        {

            #region Initial checks

            if (SampledValues.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(SampledValues),  "The given enumeration of sampled values must not be null or empty!");

            #endregion

            this.Timestamp      = Timestamp;
            this.SampledValues  = SampledValues;


            unchecked
            {

                hashCode = this.Timestamp.    GetHashCode() * 3 ^
                           this.SampledValues.CalcHashCode();

            }

        }

        #endregion


        #region Documentation

        // <ns:meterValue>
        //
        //    <ns:timestamp>?</ns:timestamp>
        //
        //    <!--1 or more repetitions:-->
        //    <ns:sampledValue>
        //
        //       <ns:value>?</ns:value>
        //
        //       <!--Optional:-->
        //       <ns:context>?</ns:context>
        //
        //       <!--Optional:-->
        //       <ns:format>?</ns:format>
        //
        //       <!--Optional:-->
        //       <ns:measurand>?</ns:measurand>
        //
        //       <!--Optional:-->
        //       <ns:phase>?</ns:phase>
        //
        //       <!--Optional:-->
        //       <ns:location>?</ns:location>
        //
        //       <!--Optional:-->
        //       <ns:unit>?</ns:unit>
        //
        //    </ns:sampledValue>
        //
        // </ns:meterValue>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:MeterValue",
        //     "title":    "MeterValue",
        //     "type":     "object",
        //     "properties": {
        //         "timestamp": {
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "sampledValue": {
        //             "type": "array",
        //             "items": {
        //                 "type": "object",
        //                 "properties": {
        //                     "value": {
        //                         "type": "string"
        //                     },
        //                     "context": {
        //                         "type": "string",
        //                         "additionalProperties": false,
        //                         "enum": [
        //                             "Interruption.Begin",
        //                             "Interruption.End",
        //                             "Sample.Clock",
        //                             "Sample.Periodic",
        //                             "Transaction.Begin",
        //                             "Transaction.End",
        //                             "Trigger",
        //                             "Other"
        //                         ]
        //                     },  
        //                     "format": {
        //                         "type": "string",
        //                         "additionalProperties": false,
        //                         "enum": [
        //                             "Raw",
        //                             "SignedData"
        //                         ]
        //                     },
        //                     "measurand": {
        //                         "type": "string",
        //                         "additionalProperties": false,
        //                         "enum": [
        //                             "Energy.Active.Export.Register",
        //                             "Energy.Active.Import.Register",
        //                             "Energy.Reactive.Export.Register",
        //                             "Energy.Reactive.Import.Register",
        //                             "Energy.Active.Export.Interval",
        //                             "Energy.Active.Import.Interval",
        //                             "Energy.Reactive.Export.Interval",
        //                             "Energy.Reactive.Import.Interval",
        //                             "Power.Active.Export",
        //                             "Power.Active.Import",
        //                             "Power.Offered",
        //                             "Power.Reactive.Export",
        //                             "Power.Reactive.Import",
        //                             "Power.Factor",
        //                             "Current.Import",
        //                             "Current.Export",
        //                             "Current.Offered",
        //                             "Voltage",
        //                             "Frequency",
        //                             "Temperature",
        //                             "SoC",
        //                             "RPM"
        //                         ]
        //                     },
        //                     "phase": {
        //                         "type": "string",
        //                         "additionalProperties": false,
        //                         "enum": [
        //                             "L1",
        //                             "L2",
        //                             "L3",
        //                             "N",
        //                             "L1-N",
        //                             "L2-N",
        //                             "L3-N",
        //                             "L1-L2",
        //                             "L2-L3",
        //                             "L3-L1"
        //                         ]
        //                     },
        //                     "location": {
        //                         "type": "string",
        //                         "additionalProperties": false,
        //                         "enum": [
        //                             "Cable",
        //                             "EV",
        //                             "Inlet",
        //                             "Outlet",
        //                             "Body"
        //                         ]
        //                     },
        //                     "unit": {
        //                         "type": "string",
        //                         "additionalProperties": false,
        //                         "enum": [
        //                             "Wh",
        //                             "kWh",
        //                             "varh",
        //                             "kvarh",
        //                             "W",
        //                             "kW",
        //                             "VA",
        //                             "kVA",
        //                             "var",
        //                             "kvar",
        //                             "A",
        //                             "V",
        //                             "K",
        //                             "Celcius",
        //                             "Fahrenheit",
        //                             "Percent"
        //                         ]
        //                     }
        //                 },
        //                 "additionalProperties": false,
        //                 "required": [
        //                     "value"
        //                 ]
        //             },
        //             "additionalProperties": false,
        //             "required": [
        //                 "timestamp",
        //                 "sampledValue"
        //             ]
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (XML)

        /// <summary>
        /// Parse the given XML representation of a meter value.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        public static MeterValue Parse(XElement XML, OnExceptionDelegate? OnException = null)
        {

            if (TryParse(XML,
                         out var meterValue,
                         out var errorResponse))
            {
                return meterValue;
            }

            throw new ArgumentException("The given XML representation of a MeterValue is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, CustomMeterValueParser = null)

        /// <summary>
        /// Parse the given JSON representation of a meter value.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomMeterValueParser">An optional delegate to parse custom MeterValues.</param>
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

            throw new ArgumentException("The given JSON representation of a MeterValue is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  out MeterValue, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a meter value.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="MeterValue">The parsed meter value.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                              XML,
                                       [NotNullWhen(true)]  out MeterValue?  MeterValue,
                                       [NotNullWhen(false)] out String?      ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                MeterValue = new MeterValue(

                                 XML.MapValueOrFail   (OCPPNS.OCPPv1_6_CS + "timestamp",
                                                       DateTime.Parse),

                                 XML.MapElementsOrFail(OCPPNS.OCPPv1_6_CS + "sampledValue",
                                                       SampledValue.Parse)

                             );

                return true;

            }
            catch (Exception e)
            {
                MeterValue     = null;
                ErrorResponse  = "The given XML representation of a MeterValue is invalid: " + e.Message;
                return false;
            }

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
        /// <param name="CustomMeterValueParser">An optional delegate to parse custom MeterValues.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out MeterValue?      MeterValue,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<MeterValue>?  CustomMeterValueParser)
        {

            try
            {

                MeterValue = null;

                #region Timestamp        [mandatory]

                if (!JSON.ParseMandatory("timestamp",
                                         "timestamp",
                                         out DateTime Timestamp,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SampledValues    [optional]

                if (JSON.ParseOptionalJSON("sampledValue",
                                           "sampled values",
                                           SampledValue.TryParse,
                                           out IEnumerable<SampledValue> SampledValues,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                MeterValue = new MeterValue(
                                 Timestamp,
                                 SampledValues
                             );

                if (CustomMeterValueParser is not null)
                    MeterValue = CustomMeterValueParser(JSON,
                                                        MeterValue);

                return true;

            }
            catch (Exception e)
            {
                MeterValue     = null;
                ErrorResponse  = "The given JSON representation of a MeterValue is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML (XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CS:meterValue"]</param>
        public XElement ToXML(XName? XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CS + "meterValue",

                   new XElement(OCPPNS.OCPPv1_6_CS + "timestamp", Timestamp.ToISO8601()),

                   SampledValues.Select(value => value.ToXML())

               );

        #endregion

        #region ToJSON(CustomMeterValueSerializer = null, CustomSampledValueSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterValueSerializer">A delegate to serialize custom meter values.</param>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterValue>?    CustomMeterValueSerializer     = null,
                              CustomJObjectSerializerDelegate<SampledValue>?  CustomSampledValueSerializer   = null)
        {

            var json = JSONObject.Create(
                           new JProperty("timestamp",      Timestamp.ToISO8601()),
                           new JProperty("sampledValue",   new JArray(SampledValues.Select(sampledValue => sampledValue.ToJSON(CustomSampledValueSerializer))))
                       );

            return CustomMeterValueSerializer is not null
                       ? CustomMeterValueSerializer(this, json)
                       : json;

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

               Timestamp.            Equals(MeterValue.Timestamp) &&

               SampledValues.Count().Equals(MeterValue.SampledValues.Count()) &&
               SampledValues.All(sampledValue => MeterValue.SampledValues.Contains(sampledValue));

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

            => String.Concat(Timestamp.ToISO8601(),
                             " / ",
                             SampledValues.Count(),
                             " sampled values");

        #endregion

    }

}
