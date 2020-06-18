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
using System.Xml.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// An OCPP charging schedule period.
    /// </summary>
    public struct ChargingSchedulePeriod : IEquatable<ChargingSchedulePeriod>
    {

        #region Properties

        /// <summary>
        /// The start of the period, in seconds from the start of schedule.
        /// This value also defines the stop time of the previous period.
        /// </summary>
        public UInt32   StartPeriod    { get; }

        /// <summary>
        /// Power limit during the schedule period in Amperes.
        /// </summary>
        public Decimal  Limit          { get; }

        /// <summary>
        /// The number of phases that can be used for charging.
        /// </summary>
        public Byte     NumberPhases   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new OCPP charging schedule period.
        /// </summary>
        /// <param name="StartPeriod">The start of the period, in seconds from the start of schedule. This value also defines the stop time of the previous period.</param>
        /// <param name="Limit">Power limit during the schedule period in Amperes.</param>
        /// <param name="NumberPhases">The number of phases that can be used for charging.</param>
        public ChargingSchedulePeriod(UInt32   StartPeriod,
                                      Decimal  Limit,
                                      Byte     NumberPhases = 3)
        {

            this.StartPeriod   = StartPeriod;
            this.Limit         = Limit;
            this.NumberPhases  = NumberPhases;

        }

        #endregion


        #region Documentation

        // <ns:chargingSchedulePeriod>
        //
        //    <ns:startPeriod>?</ns:startPeriod>
        //    <ns:limit>?</ns:limit>
        //
        //    <!--Optional:-->
        //    <ns:numberPhases>?</ns:numberPhases>
        //
        // </ns:chargingSchedulePeriod>

        #endregion

        #region (static) Parse(ChargingSchedulePeriodXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of an OCPP charging schedule period.
        /// </summary>
        /// <param name="ChargingSchedulePeriodXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingSchedulePeriod Parse(XElement             ChargingSchedulePeriodXML,
                                       OnExceptionDelegate  OnException = null)
        {

            ChargingSchedulePeriod _ChargingSchedulePeriod;

            if (TryParse(ChargingSchedulePeriodXML, out _ChargingSchedulePeriod, OnException))
                return _ChargingSchedulePeriod;

            return default(ChargingSchedulePeriod);

        }

        #endregion

        #region (static) Parse(ChargingSchedulePeriodText, OnException = null)

        /// <summary>
        /// Parse the given text representation of an OCPP charging schedule period.
        /// </summary>
        /// <param name="ChargingSchedulePeriodText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingSchedulePeriod Parse(String               ChargingSchedulePeriodText,
                                       OnExceptionDelegate  OnException = null)
        {

            ChargingSchedulePeriod _ChargingSchedulePeriod;

            if (TryParse(ChargingSchedulePeriodText, out _ChargingSchedulePeriod, OnException))
                return _ChargingSchedulePeriod;

            return default(ChargingSchedulePeriod);

        }

        #endregion

        #region (static) TryParse(ChargingSchedulePeriodXML,  out ChargingSchedulePeriod, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of an OCPP charging schedule period.
        /// </summary>
        /// <param name="ChargingSchedulePeriodXML">The XML to be parsed.</param>
        /// <param name="ChargingSchedulePeriod">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement             ChargingSchedulePeriodXML,
                                       out ChargingSchedulePeriod       ChargingSchedulePeriod,
                                       OnExceptionDelegate  OnException  = null)
        {

            try
            {

                ChargingSchedulePeriod = new ChargingSchedulePeriod(

                                             ChargingSchedulePeriodXML.MapValueOrFail   (OCPPNS.OCPPv1_6_CP + "startPeriod",
                                                                                         UInt32.Parse),

                                             ChargingSchedulePeriodXML.MapValueOrFail   (OCPPNS.OCPPv1_6_CP + "limit",
                                                                                         Decimal.Parse),

                                             ChargingSchedulePeriodXML.MapValueOrDefault(OCPPNS.OCPPv1_6_CP + "numberPhases",
                                                                                         Byte.Parse,
                                                                                         (Byte) 3)

                                         );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ChargingSchedulePeriodXML, e);

                ChargingSchedulePeriod = default(ChargingSchedulePeriod);
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ChargingSchedulePeriodText, out ChargingSchedulePeriod, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of an OCPP charging schedule period.
        /// </summary>
        /// <param name="ChargingSchedulePeriodText">The text to be parsed.</param>
        /// <param name="ChargingSchedulePeriod">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(String                      ChargingSchedulePeriodText,
                                       out ChargingSchedulePeriod  ChargingSchedulePeriod,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                if (TryParse(XDocument.Parse(ChargingSchedulePeriodText).Root,
                             out ChargingSchedulePeriod,
                             OnException))

                    return true;

            }
            catch (Exception e)
            {
                OnException?.Invoke(DateTime.UtcNow, ChargingSchedulePeriodText, e);
            }

            ChargingSchedulePeriod = default(ChargingSchedulePeriod);
            return false;

        }

        #endregion

        #region ToXML(XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CP:chargingSchedulePeriod"]</param>
        public XElement ToXML(XName XName = null)

            => new XElement(XName ?? OCPPNS.OCPPv1_6_CP + "chargingSchedulePeriod",

                   new XElement(OCPPNS.OCPPv1_6_CP + "startPeriod",   StartPeriod),
                   new XElement(OCPPNS.OCPPv1_6_CP + "limit",         Limit.ToString("0.#")),
                   new XElement(OCPPNS.OCPPv1_6_CP + "numberPhases",  NumberPhases)

               );

        #endregion


        #region Operator overloading

        #region Operator == (ChargingSchedulePeriod1, ChargingSchedulePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingSchedulePeriod1">An id tag info.</param>
        /// <param name="ChargingSchedulePeriod2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingSchedulePeriod ChargingSchedulePeriod1, ChargingSchedulePeriod ChargingSchedulePeriod2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(ChargingSchedulePeriod1, ChargingSchedulePeriod2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ChargingSchedulePeriod1 == null) || ((Object) ChargingSchedulePeriod2 == null))
                return false;

            if ((Object) ChargingSchedulePeriod1 == null)
                throw new ArgumentNullException(nameof(ChargingSchedulePeriod1),  "The given id tag info must not be null!");

            return ChargingSchedulePeriod1.Equals(ChargingSchedulePeriod2);

        }

        #endregion

        #region Operator != (ChargingSchedulePeriod1, ChargingSchedulePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingSchedulePeriod1">An id tag info.</param>
        /// <param name="ChargingSchedulePeriod2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingSchedulePeriod ChargingSchedulePeriod1, ChargingSchedulePeriod ChargingSchedulePeriod2)
            => !(ChargingSchedulePeriod1 == ChargingSchedulePeriod2);

        #endregion

        #endregion

        #region IEquatable<ChargingSchedulePeriod> Members

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
            if (!(Object is ChargingSchedulePeriod))
                return false;

            return this.Equals((ChargingSchedulePeriod) Object);

        }

        #endregion

        #region Equals(ChargingSchedulePeriod)

        /// <summary>
        /// Compares two id tag infos for equality.
        /// </summary>
        /// <param name="ChargingSchedulePeriod">An id tag info to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ChargingSchedulePeriod ChargingSchedulePeriod)
        {

            if ((Object) ChargingSchedulePeriod == null)
                return false;

            return StartPeriod. Equals(ChargingSchedulePeriod.StartPeriod) &&
                   Limit.       Equals(ChargingSchedulePeriod.Limit)       &&
                   NumberPhases.Equals(ChargingSchedulePeriod.NumberPhases);

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

                return StartPeriod. GetHashCode() * 17 ^
                       Limit.       GetHashCode() * 11 ^
                       NumberPhases.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(StartPeriod,
                             " / ",
                             Limit, " Ampere /",
                             NumberPhases, " phases");

        #endregion


    }

}
