/*
 * Copyright (c) 2014-2016 GraphDefined GmbH
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
    /// An OCPP charging schedule.
    /// </summary>
    public class ChargingSchedule
    {

        #region Properties

        /// <summary>
        /// The unit of measure the limit is expressed in.
        /// </summary>
        public ChargingRateUnitTypes                ChargingRateUnit          { get; }

        /// <summary>
        /// An enumeration of ChargingSchedulePeriods defining maximum power or current
        /// usage over time.
        /// </summary>
        public IEnumerable<ChargingSchedulePeriod>  ChargingSchedulePeriods   { get; }

        /// <summary>
        /// The optional duration of the charging schedule in seconds. If the duration is
        /// no defined, the last period will continue indefinitely or until end of the
        /// transaction in case startSchedule is also undefined.
        /// </summary>
        public UInt32?                              Duration                  { get; }

        /// <summary>
        /// The optional starting point of an absolute schedule. If absent the schedule
        /// will be relative to start of charging.
        /// </summary>
        public DateTime?                            StartSchedule             { get; }

        /// <summary>
        /// An optional minimum charging rate supported by the electric vehicle. The unit
        /// of measure is defined by the chargingRateUnit. This parameter is intended to
        /// be used by a local smart charging algorithm to optimize the power allocation
        /// for in the case a charging process is inefficient at lower charging rates.
        /// </summary>
        public Decimal?                             MinChargingRate           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP charging schedule.
        /// </summary>
        /// <param name="ChargingRateUnit">The unit of measure the limit is expressed in.</param>
        /// <param name="ChargingSchedulePeriods">An enumeration of ChargingSchedulePeriods defining maximum power or current usage over time.</param>
        /// <param name="Duration">The optional duration of the charging schedule in seconds. If the duration is no defined, the last period will continue indefinitely or until end of the transaction in case startSchedule is also undefined.</param>
        /// <param name="StartSchedule">The optional starting point of an absolute schedule. If absent the schedule will be relative to start of charging.</param>
        /// <param name="MinChargingRate">An optional minimum charging rate supported by the electric vehicle. The unit of measure is defined by the chargingRateUnit. This parameter is intended to be used by a local smart charging algorithm to optimize the power allocation for in the case a charging process is inefficient at lower charging rates.</param>
        public ChargingSchedule(ChargingRateUnitTypes                ChargingRateUnit,
                                IEnumerable<ChargingSchedulePeriod>  ChargingSchedulePeriods,
                                UInt32?                              Duration         = null,
                                DateTime?                            StartSchedule    = null,
                                Decimal?                             MinChargingRate  = null)
        {

            #region Initial checks

            if (ChargingSchedulePeriods.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(ChargingSchedulePeriods),  "The given enumeration of charging schedule periods must not be null or empty!");

            #endregion

            this.ChargingRateUnit         = ChargingRateUnit;
            this.ChargingSchedulePeriods  = ChargingSchedulePeriods;
            this.Duration                 = Duration;
            this.StartSchedule            = StartSchedule;
            this.MinChargingRate          = MinChargingRate;

        }

        #endregion


        #region Documentation

        // <ns:chargingSchedule>
        //
        //    <!--Optional:-->
        //    <ns:duration>?</ns:duration>
        //
        //    <!--Optional:-->
        //    <ns:startSchedule>?</ns:startSchedule>
        //    <ns:chargingRateUnit>?</ns:chargingRateUnit>
        //
        //    <!--1 or more repetitions:-->
        //    <ns:chargingSchedulePeriod>
        //
        //       <ns:startPeriod>?</ns:startPeriod>
        //       <ns:limit>?</ns:limit>
        //
        //       <!--Optional:-->
        //       <ns:numberPhases>?</ns:numberPhases>
        //
        //    </ns:chargingSchedulePeriod>
        //
        //    <!--Optional:-->
        //    <ns:minChargingRate>?</ns:minChargingRate>
        //
        // </ns:chargingSchedule>

        #endregion

        #region (static) Parse(ChargingScheduleXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP charging schedule.
        /// </summary>
        /// <param name="ChargingScheduleXML">The XML to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingSchedule Parse(XElement             ChargingScheduleXML,
                                             OnExceptionDelegate  OnException = null)
        {

            ChargingSchedule _ChargingSchedule;

            if (TryParse(ChargingScheduleXML, out _ChargingSchedule, OnException))
                return _ChargingSchedule;

            return null;

        }

        #endregion

        #region (static) Parse(ChargingScheduleText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP charging schedule.
        /// </summary>
        /// <param name="ChargingScheduleText">The text to parse.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingSchedule Parse(String               ChargingScheduleText,
                                             OnExceptionDelegate  OnException = null)
        {

            ChargingSchedule _ChargingSchedule;

            if (TryParse(ChargingScheduleText, out _ChargingSchedule, OnException))
                return _ChargingSchedule;

            return null;

        }

        #endregion

        #region (static) TryParse(ChargingScheduleXML,  out ChargingSchedule, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP charging schedule.
        /// </summary>
        /// <param name="ChargingScheduleXML">The XML to parse.</param>
        /// <param name="ChargingSchedule">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement              ChargingScheduleXML,
                                       out ChargingSchedule  ChargingSchedule,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                ChargingSchedule = new ChargingSchedule(

                                       ChargingScheduleXML.MapEnumValuesOrFail(OCPPNS.OCPPv1_6_CP + "chargingRateUnit",
                                                                               XML_IO.AsChargingRateUnitType),

                                       ChargingScheduleXML.MapElementsOrFail  (OCPPNS.OCPPv1_6_CP + "chargingSchedulePeriod",
                                                                               ChargingSchedulePeriod.Parse),

                                       ChargingScheduleXML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "duration",
                                                                               UInt32.Parse),

                                       ChargingScheduleXML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "startSchedule",
                                                                               DateTime.Parse),

                                       ChargingScheduleXML.MapValueOrNullable (OCPPNS.OCPPv1_6_CP + "minChargingRate ",
                                                                               Decimal.Parse)

                                   );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.Now, ChargingScheduleXML, e);

                ChargingSchedule = null;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ChargingScheduleText, out ChargingSchedule, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP charging schedule.
        /// </summary>
        /// <param name="ChargingScheduleText">The text to parse.</param>
        /// <param name="ChargingSchedule">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                ChargingScheduleText,
                                       out ChargingSchedule  ChargingSchedule,
                                       OnExceptionDelegate   OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ChargingScheduleText).Root,
                             out ChargingSchedule,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.Now, ChargingScheduleText, e);
            }

            ChargingSchedule = null;
            return false;

        }

        #endregion

        #region ToXML(XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CP:chargingSchedule"]</param>
        public XElement ToXML(XName XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CP + "chargingSchedule",

                   Duration.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "duration",         Duration.Value)
                       : null,

                   StartSchedule.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "startSchedule",    StartSchedule.Value.ToIso8601())
                       : null,

                   new XElement(OCPPNS.OCPPv1_6_CP + "chargingRateUnit",       XML_IO.AsText(ChargingRateUnit)),

                   ChargingSchedulePeriods.Select(value => value.ToXML()),

                   Duration.HasValue
                       ? new XElement(OCPPNS.OCPPv1_6_CP + "minChargingRate",  MinChargingRate.Value.ToString("0.#"))
                       : null

               );

        #endregion


        #region Operator overloading

        #region Operator == (ChargingSchedule1, ChargingSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingSchedule1">An id tag info.</param>
        /// <param name="ChargingSchedule2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingSchedule ChargingSchedule1, ChargingSchedule ChargingSchedule2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ChargingSchedule1, ChargingSchedule2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChargingSchedule1 == null) || ((Object) ChargingSchedule2 == null))
                return false;

            if ((Object) ChargingSchedule1 == null)
                throw new ArgumentNullException(nameof(ChargingSchedule1),  "The given id tag info must not be null!");

            return ChargingSchedule1.Equals(ChargingSchedule2);

        }

        #endregion

        #region Operator != (ChargingSchedule1, ChargingSchedule2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingSchedule1">An id tag info.</param>
        /// <param name="ChargingSchedule2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingSchedule ChargingSchedule1, ChargingSchedule ChargingSchedule2)
            => !(ChargingSchedule1 == ChargingSchedule2);

        #endregion

        #endregion

        #region IEquatable<ChargingSchedule> Members

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
            var ChargingSchedule = Object as ChargingSchedule;
            if ((Object) ChargingSchedule == null)
                return false;

            return this.Equals(ChargingSchedule);

        }

        #endregion

        #region Equals(ChargingSchedule)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="ChargingSchedule">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingSchedule ChargingSchedule)
        {

            if ((Object) ChargingSchedule == null)
                return false;

            return ChargingRateUnit.Equals(ChargingSchedule.ChargingRateUnit) &&

                   ChargingSchedulePeriods.Count().Equals(ChargingSchedule.ChargingSchedulePeriods.Count()) &&

                   ((!Duration.       HasValue && !ChargingSchedule.Duration.       HasValue) ||
                     (Duration.       HasValue &&  ChargingSchedule.Duration.       HasValue && Duration.       Value.Equals(ChargingSchedule.Duration.       Value))) &&

                   ((!StartSchedule.  HasValue && !ChargingSchedule.StartSchedule.  HasValue) ||
                     (StartSchedule.  HasValue &&  ChargingSchedule.StartSchedule.  HasValue && StartSchedule.  Value.Equals(ChargingSchedule.StartSchedule.  Value))) &&

                   ((!MinChargingRate.HasValue && !ChargingSchedule.MinChargingRate.HasValue) ||
                     (MinChargingRate.HasValue &&  ChargingSchedule.MinChargingRate.HasValue && MinChargingRate.Value.Equals(ChargingSchedule.MinChargingRate.Value)));

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

                return ChargingRateUnit.       GetHashCode() * 19 ^
                       ChargingSchedulePeriods.GetHashCode() * 17 ^

                       (Duration.HasValue
                            ? Duration.       GetHashCode() * 11
                            : 0) ^

                       (StartSchedule.HasValue
                            ? StartSchedule.  GetHashCode() * 7
                            : 0) ^

                       (MinChargingRate.HasValue
                            ? MinChargingRate.GetHashCode() * 5
                            : 0);

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ChargingRateUnit,
                             " / ",
                             ChargingSchedulePeriods.Count(),
                             " charging schedule period(s)");

        #endregion


    }

}
