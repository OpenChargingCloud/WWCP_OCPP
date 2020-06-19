/*
 * Copyright (c) 2014-2020 GraphDefined GmbH
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
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
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
        /// Create an new meter value.
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

        #region (static) Parse   (MeterValueXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a meter value.
        /// </summary>
        /// <param name="MeterValueXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValue Parse(XElement             MeterValueXML,
                                       OnExceptionDelegate  OnException = null)
        {

            if (TryParse(MeterValueXML,
                         out MeterValue meterValue,
                         OnException))
            {
                return meterValue;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (MeterValueJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a meter value.
        /// </summary>
        /// <param name="MeterValueJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValue Parse(JObject              MeterValueJSON,
                                       OnExceptionDelegate  OnException = null)
        {

            if (TryParse(MeterValueJSON,
                         out MeterValue meterValue,
                         OnException))
            {
                return meterValue;
            }

            return null;

        }

        #endregion

        #region (static) Parse   (MeterValueText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a meter value.
        /// </summary>
        /// <param name="MeterValueText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValue Parse(String               MeterValueText,
                                       OnExceptionDelegate  OnException = null)
        {

            if (TryParse(MeterValueText,
                         out MeterValue meterValue,
                         OnException))
            {
                return meterValue;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(MeterValueXML,  out MeterValue, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a meter value.
        /// </summary>
        /// <param name="MeterValueXML">The XML to be parsed.</param>
        /// <param name="MeterValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement             MeterValueXML,
                                       out MeterValue       MeterValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                MeterValue = new MeterValue(

                                    MeterValueXML.MapValueOrFail   (OCPPNS.OCPPv1_6_CS + "timestamp",
                                                                    DateTime.Parse),

                                    MeterValueXML.MapElementsOrFail(OCPPNS.OCPPv1_6_CS + "sampledValue",
                                                                    SampledValue.Parse)

                                );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, MeterValueXML, e);

                MeterValue = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(MeterValueJSON, out MeterValue, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a meter value.
        /// </summary>
        /// <param name="MeterValueJSON">The JSON to be parsed.</param>
        /// <param name="MeterValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject              MeterValueJSON,
                                       out MeterValue       MeterValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                MeterValue = null;

                #region Timestamp

                if (!MeterValueJSON.ParseMandatory("timestamp",
                                                   "timestamp",
                                                   out DateTime  Timestamp,
                                                   out String    ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region SampledValues

                var SampledValues = new List<SampledValue>();

                if (MeterValueJSON.ParseOptional("sampledValue",
                                                 "sampled values",
                                                 out JArray  SampledValuesJSON,
                                                 out         ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (SampledValuesJSON.SafeAny())
                    {
                        foreach (var sampledValueJSON in SampledValuesJSON)
                        {

                            if (sampledValueJSON is JObject &&
                                SampledValue.TryParse(sampledValueJSON as JObject, out SampledValue sampledValue))
                            {
                                SampledValues.Add(sampledValue);
                            }

                            else
                                return false;

                        }
                    }

                }

                #endregion


                MeterValue = new MeterValue(Timestamp,
                                            SampledValues);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, MeterValueJSON, e);

                MeterValue = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(MeterValueText, out MeterValue, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a meter value.
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

                if (MeterValueText.IsNotNullOrEmpty())
                {

                    if (MeterValueText.StartsWith("{") &&
                        TryParse(JObject.Parse(MeterValueText),
                                 out MeterValue,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(MeterValueText).Root,
                                 out MeterValue,
                                 OnException))
                    {
                        return true;
                    }

                }

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, MeterValueText, e);
            }

            MeterValue = null;
            return false;

        }

        #endregion

        #region ToXML(XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CS:meterValue"]</param>
        public XElement ToXML(XName XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CS + "meterValue",

                   new XElement(OCPPNS.OCPPv1_6_CS + "timestamp", Timestamp.ToIso8601()),

                   SampledValues.Select(value => value.ToXML())

               );

        #endregion

        #region ToJSON(CustomMeterValueSerializer = null, CustomSampledValueSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomMeterValueSerializer">A delegate to serialize custom meter values.</param>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<MeterValue>    CustomMeterValueSerializer     = null,
                              CustomJObjectSerializerDelegate<SampledValue>  CustomSampledValueSerializer   = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("timestamp",              Timestamp.    ToIso8601()),

                           SampledValues.SafeAny()
                               ? new JProperty("sampledValue",  new JArray(SampledValues.Select(sampledValue => sampledValue.ToJSON(CustomSampledValueSerializer))))
                               : null

                       );

            return CustomMeterValueSerializer != null
                       ? CustomMeterValueSerializer(this, JSON)
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
            if ((MeterValue1 is null) || (MeterValue2 is null))
                return false;

            if ((Object) MeterValue1 == null)
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

            return Timestamp.            Equals(MeterValue.Timestamp) &&
                   //FixMe!
                   SampledValues.Count().Equals(MeterValue.SampledValues.Count());

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

                return Timestamp.    GetHashCode() * 11 ^
                       SampledValues.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Timestamp.ToIso8601(),
                             " / ",
                             SampledValues.Count(),
                             " sampled values");

        #endregion


    }

}
