/*
 * Copyright (c) 2014-2018 GraphDefined GmbH
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
using System.Xml.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP sampled value.
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
        public ReadingContexts?  Context      { get; }

        /// <summary>
        /// Raw or signed data.
        /// Default = “Raw”.
        /// </summary>
        public ValueFormats?     Format       { get; }

        /// <summary>
        /// Type of measurement.
        /// Default = “Energy.Active.Import.Register”.
        /// </summary>
        public Measurands?       Measurand    { get; }

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
        public Locations?        Location     { get; }

        /// <summary>
        /// Unit of the value.
        /// Default = “Wh” if the (default) measurand is an “Energy” type.
        /// </summary>
        public UnitsOfMeasure?   Unit         { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP sampled value.
        /// </summary>
        /// <param name="Value">Value as a “Raw” (decimal) number or “SignedData”. Field Type is “string” to allow for digitally signed data readings.</param>
        /// <param name="Context">Type of detail value: start, end or sample. Default = “Sample.Periodic”.</param>
        /// <param name="Format">Raw or signed data. Default = “Raw”.</param>
        /// <param name="Measurand">Type of measurement. Default = “Energy.Active.Import.Register”.</param>
        /// <param name="Phase">Indicates how the measured value is to be interpreted. For instance between L1 and neutral (L1-N).</param>
        /// <param name="Location">Location of measurement. Default=”Outlet”.</param>
        /// <param name="Unit">Unit of the value. Default = “Wh” if the (default) measurand is an “Energy” type.</param>
        public SampledValue(String            Value,
                            ReadingContexts?  Context,
                            ValueFormats?     Format,
                            Measurands?       Measurand,
                            Phases?           Phase,
                            Locations?        Location,
                            UnitsOfMeasure?   Unit)
        {

            #region Initial checks

            if (Value.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Value),  "The given sampled value must not be null or empty!");

            #endregion

            this.Value      = Value;

            this.Context    = Context   ?? ReadingContexts.SamplePeriodic;
            this.Format     = Format    ?? ValueFormats.Raw;
            this.Measurand  = Measurand ?? Measurands.EnergyActiveImportRegister;
            this.Phase      = Phase     ?? new Phases?();
            this.Location   = Location  ?? Locations.Outlet;
            this.Unit       = Unit      ?? new UnitsOfMeasure?();

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

        #endregion

        #region (static) Parse(SampledValueXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP sampled value.
        /// </summary>
        /// <param name="SampledValueXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SampledValue Parse(XElement             SampledValueXML,
                                         OnExceptionDelegate  OnException = null)
        {

            SampledValue _SampledValue;

            if (TryParse(SampledValueXML, out _SampledValue, OnException))
                return _SampledValue;

            return null;

        }

        #endregion

        #region (static) Parse(SampledValueText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP sampled value.
        /// </summary>
        /// <param name="SampledValueText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static SampledValue Parse(String               SampledValueText,
                                         OnExceptionDelegate  OnException = null)
        {

            SampledValue _SampledValue;

            if (TryParse(SampledValueText, out _SampledValue, OnException))
                return _SampledValue;

            return null;

        }

        #endregion

        #region (static) TryParse(SampledValueXML,  out SampledValue, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP sampled value.
        /// </summary>
        /// <param name="SampledValueXML">The XML to parse.</param>
        /// <param name="SampledValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement             SampledValueXML,
                                       out SampledValue     SampledValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                SampledValue = new SampledValue(

                                   SampledValueXML.ElementValueOrFail(OCPPNS.OCPPv1_6_CS + "value"),

                                   SampledValueXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "context",
                                                                      XML_IO.AsReadingContexts),

                                   SampledValueXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "format",
                                                                      XML_IO.AsValueFormats),

                                   SampledValueXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "measurand",
                                                                      XML_IO.AsMeasurand),

                                   SampledValueXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "phase",
                                                                      XML_IO.AsPhases),

                                   SampledValueXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "location",
                                                                      XML_IO.AsLocations),

                                   SampledValueXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CS + "unit",
                                                                      XML_IO.AsUnitsOfMeasure)

                               );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, SampledValueXML, e);

                SampledValue = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(SampledValueText, out SampledValue, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP sampled value.
        /// </summary>
        /// <param name="SampledValueText">The text to parse.</param>
        /// <param name="SampledValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               SampledValueText,
                                       out SampledValue     SampledValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(SampledValueText).Root,
                             out SampledValue,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, SampledValueText, e);
            }

            SampledValue = null;
            return false;

        }

        #endregion

        #region ToXML(XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CS:idTagInfo"]</param>
        public XElement ToXML(XName XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CS + "sampledValue",

                   new XElement(OCPPNS.OCPPv1_6_CS + "value",  Value),

                   Context.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "context",   XML_IO.AsText(Context.  Value))
                       : null,

                   Format.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "format",    XML_IO.AsText(Format.   Value))
                       : null,

                   Measurand.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "measurand", XML_IO.AsText(Measurand.Value))
                       : null,

                   Phase.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "phase",     XML_IO.AsText(Phase.    Value))
                       : null,

                   Location.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "location",  XML_IO.AsText(Location. Value))
                       : null,

                   Unit.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CS + "unit",      XML_IO.AsText(Unit.     Value))
                       : null

               );

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
            if (Object.ReferenceEquals(SampledValue1, SampledValue2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) SampledValue1 == null) || ((Object) SampledValue2 == null))
                return false;

            if ((Object) SampledValue1 == null)
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

            if (Object == null)
                return false;

            // Check if the given object is a id tag info.
            var SampledValue = Object as SampledValue;
            if ((Object) SampledValue == null)
                return false;

            return this.Equals(SampledValue);

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

            if ((Object) SampledValue == null)
                return false;

            return Value.Equals(SampledValue.Value) &&

                   ((!Context.  HasValue && !SampledValue.Context.  HasValue) ||
                     (Context.  HasValue &&  SampledValue.Context.  HasValue && Context.  Value.Equals(SampledValue.Context.  Value))) &&

                   ((!Format.   HasValue && !SampledValue.Format.   HasValue) ||
                     (Format.   HasValue &&  SampledValue.Format.   HasValue && Format.   Value.Equals(SampledValue.Format.   Value))) &&

                   ((!Measurand.HasValue && !SampledValue.Measurand.HasValue) ||
                     (Measurand.HasValue &&  SampledValue.Measurand.HasValue && Measurand.Value.Equals(SampledValue.Measurand.Value))) &&

                   ((!Phase.    HasValue && !SampledValue.Phase.    HasValue) ||
                     (Phase.    HasValue &&  SampledValue.Phase.    HasValue && Phase.    Value.Equals(SampledValue.Phase.    Value))) &&

                   ((!Location. HasValue && !SampledValue.Location. HasValue) ||
                     (Location. HasValue &&  SampledValue.Location. HasValue && Location. Value.Equals(SampledValue.Location. Value))) &&

                   ((!Unit.     HasValue && !SampledValue.Unit.     HasValue) ||
                     (Unit.     HasValue &&  SampledValue.Unit.     HasValue && Unit.     Value.Equals(SampledValue.Unit.     Value)));

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

                return Value.GetHashCode() * 31 ^

                       (Context.  HasValue
                            ? Context.  GetHashCode() * 23
                            : 0) ^

                       (Format.   HasValue
                            ? Format.   GetHashCode() * 17
                            : 0) ^

                       (Measurand.HasValue
                            ? Measurand.GetHashCode() * 13
                            : 0) ^

                       (Phase.    HasValue
                            ? Phase.    GetHashCode() * 11
                            : 0) ^

                       (Location. HasValue
                            ? Location. GetHashCode() *  7
                            : 0) ^

                       (Unit.     HasValue
                            ? Unit.     GetHashCode() *  5
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Value,

                             Unit.HasValue
                                 ? " " + Unit.Value
                                 : "",

                             Location.HasValue
                                 ? " measured at " + Location.Value
                                 : "");

        #endregion


    }

}
