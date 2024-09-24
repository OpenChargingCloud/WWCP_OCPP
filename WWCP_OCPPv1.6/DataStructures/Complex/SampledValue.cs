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

using System.Xml.Linq;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace cloud.charging.open.protocols.OCPPv1_6
{

    /// <summary>
    /// A sampled value.
    /// </summary>
    public class SampledValue : IEquatable<SampledValue>
    {

        #region Properties

        /// <summary>
        /// Value as a “Raw” (decimal) number or “SignedData”.
        /// Field Type is “string” to allow for digitally signed data readings.
        /// </summary>
        public String            Value        { get; }

        /// <summary>
        /// Type of detail value: start, end or sample.
        /// Default = “Sample.Periodic”.
        /// </summary>
        public ReadingContexts   Context      { get; }

        /// <summary>
        /// Raw or signed data.
        /// Default = “Raw”.
        /// </summary>
        public ValueFormats      Format       { get; }

        /// <summary>
        /// Type of measurement.
        /// Default = “Energy.Active.Import.Register”.
        /// </summary>
        public Measurands        Measurand    { get; }

        /// <summary>
        /// Indicates how the measured value is to be interpreted.
        /// For instance between L1 and neutral (L1-N).
        /// Please note that not all values of phase are applicable
        /// to all measurands. When phase is absent, the measured
        /// value is interpreted as an overall value.
        /// </summary>
        public Phases?           Phase        { get; }

        /// <summary>
        /// Location of measurement. Default=”Outlet”.
        /// </summary>
        public Locations         Location     { get; }

        /// <summary>
        /// Unit of the value.
        /// Default = “Wh” if the (default) measurand is an “Energy” type.
        /// </summary>
        public UnitsOfMeasure    Unit         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new OCPP sampled value.
        /// </summary>
        /// <param name="Value">Value as a “Raw” (decimal) number or “SignedData”. Field Type is “string” to allow for digitally signed data readings.</param>
        /// <param name="Context">Type of detail value: start, end or sample. Default = “Sample.Periodic”.</param>
        /// <param name="Format">Raw or signed data. Default = “Raw”.</param>
        /// <param name="Measurand">Type of measurement. Default = “Energy.Active.Import.Register”.</param>
        /// <param name="Phase">Indicates how the measured value is to be interpreted. For instance between L1 and neutral (L1-N).</param>
        /// <param name="Location">Location of measurement. Default=”Outlet”.</param>
        /// <param name="Unit">Unit of the value. Default = “Wh” if the (default) measurand is an “Energy” type.</param>
        public SampledValue(String            Value,
                            ReadingContexts?  Context     = null,
                            ValueFormats?     Format      = null,
                            Measurands?       Measurand   = null,
                            Phases?           Phase       = null,
                            Locations?        Location    = null,
                            UnitsOfMeasure?   Unit        = null)
        {

            #region Initial checks

            Value = Value.Trim();

            if (Value.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Value),  "The given sampled value must not be null or empty!");

            #endregion

            this.Value      = Value;

            this.Context    = Context   ?? ReadingContexts.SamplePeriodic;
            this.Format     = Format    ?? ValueFormats.Raw;
            this.Measurand  = Measurand ?? Measurands.EnergyActiveImportRegister;
            this.Phase      = Phase;
            this.Location   = Location  ?? Locations.Outlet;
            this.Unit       = Unit      ?? UnitsOfMeasure.Wh;

            unchecked
            {

                hashCode = this.Value.    GetHashCode() * 17 ^
                           this.Context.  GetHashCode() * 13 ^
                           this.Format.   GetHashCode() * 11 ^
                           this.Measurand.GetHashCode() *  7 ^
                           this.Location. GetHashCode() *  5 ^
                           this.Unit.     GetHashCode() *  3 ^
                          (this.Phase?.   GetHashCode() ?? 0);

            }

        }

        #endregion


        #region Documentation

        // <ns:sampledValue>
        //
        //    <ns:value>?</ns:value>
        //
        //    <!--Optional:-->
        //    <ns:context>?</ns:context>
        //
        //    <!--Optional:-->
        //    <ns:format>?</ns:format>
        //
        //    <!--Optional:-->
        //    <ns:measurand>?</ns:measurand>
        //
        //    <!--Optional:-->
        //    <ns:phase>?</ns:phase>
        //
        //    <!--Optional:-->
        //    <ns:location>?</ns:location>
        //
        //    <!--Optional:-->
        //    <ns:unit>?</ns:unit>
        //
        // </ns:sampledValue>

        // {
        //     "$schema":  "http://json-schema.org/draft-04/schema#",
        //     "id":       "urn:OCPP:1.6:2019:12:MeterValue",
        //     "title":    "MeterValue",
        //     "type":     "object",
        //     "properties": {
        //         "value": {
        //             "type": "string"
        //         },
        //         "context": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Interruption.Begin",
        //                 "Interruption.End",
        //                 "Sample.Clock",
        //                 "Sample.Periodic",
        //                 "Transaction.Begin",
        //                 "Transaction.End",
        //                 "Trigger",
        //                 "Other"
        //             ]
        //         },  
        //         "format": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Raw",
        //                 "SignedData"
        //             ]
        //         },
        //         "measurand": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Energy.Active.Export.Register",
        //                 "Energy.Active.Import.Register",
        //                 "Energy.Reactive.Export.Register",
        //                 "Energy.Reactive.Import.Register",
        //                 "Energy.Active.Export.Interval",
        //                 "Energy.Active.Import.Interval",
        //                 "Energy.Reactive.Export.Interval",
        //                 "Energy.Reactive.Import.Interval",
        //                 "Power.Active.Export",
        //                 "Power.Active.Import",
        //                 "Power.Offered",
        //                 "Power.Reactive.Export",
        //                 "Power.Reactive.Import",
        //                 "Power.Factor",
        //                 "Current.Import",
        //                 "Current.Export",
        //                 "Current.Offered",
        //                 "Voltage",
        //                 "Frequency",
        //                 "Temperature",
        //                 "SoC",
        //                 "RPM"
        //             ]
        //         },
        //         "phase": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "L1",
        //                 "L2",
        //                 "L3",
        //                 "N",
        //                 "L1-N",
        //                 "L2-N",
        //                 "L3-N",
        //                 "L1-L2",
        //                 "L2-L3",
        //                 "L3-L1"
        //             ]
        //         },
        //         "location": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Cable",
        //                 "EV",
        //                 "Inlet",
        //                 "Outlet",
        //                 "Body"
        //             ]
        //         },
        //         "unit": {
        //             "type": "string",
        //             "additionalProperties": false,
        //             "enum": [
        //                 "Wh",
        //                 "kWh",
        //                 "varh",
        //                 "kvarh",
        //                 "W",
        //                 "kW",
        //                 "VA",
        //                 "kVA",
        //                 "var",
        //                 "kvar",
        //                 "A",
        //                 "V",
        //                 "K",
        //                 "Celcius",
        //                 "Fahrenheit",
        //                 "Percent"
        //             ]
        //         }
        //     },
        //     "additionalProperties": false,
        //     "required": [
        //         "value"
        //     ]
        // }

        #endregion

        #region (static) Parse   (XML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a sampled value.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SampledValue Parse(XElement              XML,
                                         OnExceptionDelegate?  OnException   = null)
        {

            if (TryParse(XML,
                         out var sampledValue,
                         OnException))
            {
                return sampledValue;
            }

            throw new ArgumentException("The given XML representation of a SampledValue is invalid: ", // + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, CustomSampledValueParser = null)

        /// <summary>
        /// Parse the given text representation of a sampled value.
        /// </summary>
        /// <param name="JSON">The text to be parsed.</param>
        /// <param name="CustomSampledValueParser">An optional delegate to parse custom DataTransfer requests.</param>
        public static SampledValue Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<SampledValue>?  CustomSampledValueParser   = null)
        {

            if (TryParse(JSON,
                         out var sampledValue,
                         out var errorResponse,
                         CustomSampledValueParser))
            {
                return sampledValue;
            }

            throw new ArgumentException("The given JSON representation of a SampledValue is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  out SampledValue, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a sampled value.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="SampledValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement              XML,
                                       out SampledValue?     SampledValue,
                                       OnExceptionDelegate?  OnException   = null)
        {

            try
            {

                SampledValue = new SampledValue(

                                   XML.ElementValueOrFail(OCPPNS.OCPPv1_6_CS + "value"),

                                   XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "context",
                                                          ReadingContextExtensions.Parse),

                                   XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "format",
                                                          ValueFormatExtensions.Parse),

                                   XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "measurand",
                                                          MeasurandExtensions.Parse),

                                   XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "phase",
                                                          PhasesExtensions.Parse),

                                   XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "location",
                                                          LocationExtensions.Parse),

                                   XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "unit",
                                                          UnitsOfMeasureExtensions.Parse)

                               );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(Timestamp.Now, XML, e);

                SampledValue = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, out SampledValue, out ErrorResponse, CustomSampledValueParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given text representation of a sampled value.
        /// </summary>
        /// <param name="JSON">The text to be parsed.</param>
        /// <param name="SampledValue">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                 JSON,
                                       [NotNullWhen(true)]  out SampledValue?  SampledValue,
                                       [NotNullWhen(false)] out String?        ErrorResponse)

            => TryParse(JSON,
                        out SampledValue,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given text representation of a sampled value.
        /// </summary>
        /// <param name="JSON">The text to be parsed.</param>
        /// <param name="SampledValue">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomSampledValueParser">An optional delegate to parse custom SampledValues.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       [NotNullWhen(true)]  out SampledValue?      SampledValue,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomJObjectParserDelegate<SampledValue>?  CustomSampledValueParser)
        {

            try
            {

                SampledValue = null;

                #region Value        [mandatory]

                if (!JSON.ParseMandatoryText("value",
                                             "value",
                                             out String? Value,
                                             out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Context      [optional]

                if (JSON.ParseOptional("context",
                                       "context",
                                       ReadingContextExtensions.Parse,
                                       out ReadingContexts Context,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Format       [optional]

                if (JSON.ParseOptional("format",
                                       "format",
                                       ValueFormatExtensions.Parse,
                                       out ValueFormats Format,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Measurand    [optional]

                if (JSON.ParseOptional("measurand",
                                       "measurand",
                                       MeasurandExtensions.Parse,
                                       out Measurands Measurand,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Phase        [optional]

                if (JSON.ParseOptional("phase",
                                       "phase",
                                       PhasesExtensions.Parse,
                                       out Phases Phase,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Location     [optional]

                if (JSON.ParseOptional("location",
                                       "location",
                                       LocationExtensions.Parse,
                                       out Locations Location,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Unit         [optional]

                if (JSON.ParseOptional("unit",
                                       "unit",
                                       UnitsOfMeasureExtensions.Parse,
                                       out UnitsOfMeasure Unit,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                SampledValue = new SampledValue(
                                   Value,
                                   Context,
                                   Format,
                                   Measurand,
                                   Phase,
                                   Location,
                                   Unit
                               );

                if (CustomSampledValueParser is not null)
                    SampledValue = CustomSampledValueParser(JSON,
                                                            SampledValue);

                return true;

            }
            catch (Exception e)
            {
                SampledValue   = default;
                ErrorResponse  = "The given JSON representation of a SampledValue is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToXML (XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CS:idTagInfo"]</param>
        public XElement ToXML(XName? XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CS + "sampledValue",

                   new XElement(OCPPNS.OCPPv1_6_CS + "value",  Value),
                   new XElement(OCPPNS.OCPPv1_6_CS + "context",     Context.        AsText()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "format",      Format.         AsText()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "measurand",   Measurand.      AsText()),

                   Phase.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "phase", Phase.    Value.AsText())
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CS + "location",    Location.       AsText()),
                   new XElement(OCPPNS.OCPPv1_6_CS + "unit",        Unit.           AsText())

               );

        #endregion

        #region ToJSON(CustomSampledValueSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomSampledValueSerializer">A delegate to serialize custom sampled values.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<SampledValue>?  CustomSampledValueSerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("value",       Value),

                                 new JProperty("context",     Context.    AsText()),
                                 new JProperty("format",      Format.     AsText()),
                                 new JProperty("measurand",   Measurand.  AsText()),

                           Phase.HasValue
                               ? new JProperty("phase",       Phase.Value.AsText())
                               : null,

                                 new JProperty("location",    Location.   AsText()),
                                 new JProperty("unit",        Unit.       AsText())

                       );

            return CustomSampledValueSerializer is not null
                       ? CustomSampledValueSerializer(this, json)
                       : json;

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
        /// <param name="SampledValue1">An id tag info.</param>
        /// <param name="SampledValue2">Another id tag info.</param>
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

               Value.    Equals(SampledValue.Value)     &&
               Context.  Equals(SampledValue.Context)   &&
               Format.   Equals(SampledValue.Format)    &&
               Measurand.Equals(SampledValue.Measurand) &&
               Location. Equals(SampledValue.Location)  &&
               Unit.     Equals(SampledValue.Unit)      &&

               ((!Phase.HasValue && !SampledValue.Phase.HasValue) ||
                 (Phase.HasValue &&  SampledValue.Phase.HasValue && Phase.Value.Equals(SampledValue.Phase.Value)));

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

            => $"{Value} {Unit} measured at '{Location}'";

        #endregion

    }

}
