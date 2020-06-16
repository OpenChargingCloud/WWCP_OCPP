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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP meter value.
    /// </summary>
    public class MeterValue : IEquatable<MeterValue>
    {

        #region Properties

        /// <summary>
        /// The timestamp of the measured value(s).
        /// </summary>
        public DateTime                   Timestamp       { get; }

        /// <summary>
        /// An enumeration of measured values.
        /// </summary>
        public IEnumerable<SampledValue>  SampledValues   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP meter value.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the measured value(s).</param>
        /// <param name="SampledValues">An enumeration of measured values.</param>
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


        #endregion

        #region (static) Parse(MeterValueXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP meter value.
        /// </summary>
        /// <param name="MeterValueXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValue Parse(XElement             MeterValueXML,
                                       OnExceptionDelegate  OnException = null)
        {

            MeterValue _MeterValue;

            if (TryParse(MeterValueXML, out _MeterValue, OnException))
                return _MeterValue;

            return null;

        }

        #endregion

        #region (static) Parse(MeterValueText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP meter value.
        /// </summary>
        /// <param name="MeterValueText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static MeterValue Parse(String               MeterValueText,
                                       OnExceptionDelegate  OnException = null)
        {

            MeterValue _MeterValue;

            if (TryParse(MeterValueText, out _MeterValue, OnException))
                return _MeterValue;

            return null;

        }

        #endregion

        #region (static) TryParse(MeterValueXML,  out MeterValue, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP meter value.
        /// </summary>
        /// <param name="MeterValueXML">The XML to parse.</param>
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

        #region (static) TryParse(MeterValueText, out MeterValue, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP meter value.
        /// </summary>
        /// <param name="MeterValueText">The text to parse.</param>
        /// <param name="MeterValue">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String               MeterValueText,
                                       out MeterValue       MeterValue,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(MeterValueText).Root,
                             out MeterValue,
                             OnException))

                    return true;

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
            if (((Object) MeterValue1 == null) || ((Object) MeterValue2 == null))
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

            if (Object == null)
                return false;

            // Check if the given object is a id tag info.
            var MeterValue = Object as MeterValue;
            if ((Object) MeterValue == null)
                return false;

            return this.Equals(MeterValue);

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

            if ((Object) MeterValue == null)
                return false;

            return Timestamp.Equals(MeterValue.Timestamp) &&

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
