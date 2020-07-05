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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace org.GraphDefined.WWCP.OCPPv1_6
{

    /// <summary>
    /// A charging schedule period.
    /// </summary>
    public struct ChargingSchedulePeriod : IEquatable<ChargingSchedulePeriod>
    {

        #region Properties

        /// <summary>
        /// The start of the period, in seconds from the start of schedule.
        /// This value also defines the stop time of the previous period.
        /// </summary>
        public UInt32   StartPeriod     { get; }

        /// <summary>
        /// Power limit during the schedule period in Amperes.
        /// </summary>
        public Decimal  Limit           { get; }

        /// <summary>
        /// The number of phases that can be used for charging.
        /// </summary>
        public Byte?    NumberPhases    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new charging schedule period.
        /// </summary>
        /// <param name="StartPeriod">The start of the period, in seconds from the start of schedule. This value also defines the stop time of the previous period.</param>
        /// <param name="Limit">Power limit during the schedule period in Amperes.</param>
        /// <param name="NumberPhases">The number of phases that can be used for charging.</param>
        public ChargingSchedulePeriod(UInt32   StartPeriod,
                                      Decimal  Limit,
                                      Byte?    NumberPhases  = null)
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

        // {
        //     "$schema": "http://json-schema.org/draft-04/schema#",
        //     "id":      "urn:OCPP:1.6:2019:12:RemoteStartTransactionRequest",
        //     "title":   "chargingSchedulePeriod",
        //     "type": "array",
        //     "items": {
        //         "type": "object",
        //         "properties": {
        //             "startPeriod": {
        //                 "type": "integer"
        //             },
        //             "limit": {
        //                 "type": "number",
        //                 "multipleOf" : 0.1
        //             },
        //             "numberPhases": {
        //                 "type": "integer"
        //             }
        //         },
        //         "additionalProperties": false,
        //         "required": [
        //             "startPeriod",
        //             "limit"
        //         ]
        //     }
        // }

        #endregion

        #region (static) Parse   (ChargingSchedulePeriodXML,  OnException = null)

        /// <summary>
        /// Parse the given XML representation of a charging schedule period.
        /// </summary>
        /// <param name="ChargingSchedulePeriodXML">The XML to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingSchedulePeriod Parse(XElement             ChargingSchedulePeriodXML,
                                                   OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ChargingSchedulePeriodXML,
                         out ChargingSchedulePeriod chargingSchedulePeriod,
                         OnException))
            {
                return chargingSchedulePeriod;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (ChargingSchedulePeriodJSON, OnException = null)

        /// <summary>
        /// Parse the given JSON representation of a charging schedule period.
        /// </summary>
        /// <param name="ChargingSchedulePeriodJSON">The JSON to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingSchedulePeriod Parse(JObject              ChargingSchedulePeriodJSON,
                                                   OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ChargingSchedulePeriodJSON,
                         out ChargingSchedulePeriod chargingSchedulePeriod,
                         OnException))
            {
                return chargingSchedulePeriod;
            }

            return default;

        }

        #endregion

        #region (static) Parse   (ChargingSchedulePeriodText, OnException = null)

        /// <summary>
        /// Parse the given text representation of a charging schedule period.
        /// </summary>
        /// <param name="ChargingSchedulePeriodText">The text to be parsed.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static ChargingSchedulePeriod Parse(String               ChargingSchedulePeriodText,
                                                   OnExceptionDelegate  OnException = null)
        {

            if (TryParse(ChargingSchedulePeriodText,
                         out ChargingSchedulePeriod chargingSchedulePeriod,
                         OnException))
            {
                return chargingSchedulePeriod;
            }

            return default;

        }

        #endregion

        #region (static) TryParse(ChargingSchedulePeriodXML,  out ChargingSchedulePeriod, OnException = null)

        /// <summary>
        /// Try to parse the given XML representation of a charging schedule period.
        /// </summary>
        /// <param name="ChargingSchedulePeriodXML">The XML to be parsed.</param>
        /// <param name="ChargingSchedulePeriod">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(XElement                    ChargingSchedulePeriodXML,
                                       out ChargingSchedulePeriod  ChargingSchedulePeriod,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                ChargingSchedulePeriod = new ChargingSchedulePeriod(

                                             ChargingSchedulePeriodXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "startPeriod",
                                                                                          UInt32.Parse),

                                             ChargingSchedulePeriodXML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "limit",
                                                                                          Decimal.Parse),

                                             ChargingSchedulePeriodXML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "numberPhases",
                                                                                          Byte.Parse)

                                         );

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ChargingSchedulePeriodXML, e);

                ChargingSchedulePeriod = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ChargingSchedulePeriodJSON, out ChargingSchedulePeriod, OnException = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging schedule period.
        /// </summary>
        /// <param name="ChargingSchedulePeriodJSON">The JSON to be parsed.</param>
        /// <param name="ChargingSchedulePeriod">The parsed connector type.</param>
        /// <param name="OnException">An optional delegate called whenever an exception occured.</param>
        public static Boolean TryParse(JObject                     ChargingSchedulePeriodJSON,
                                       out ChargingSchedulePeriod  ChargingSchedulePeriod,
                                       OnExceptionDelegate         OnException  = null)
        {

            try
            {

                ChargingSchedulePeriod = default;

                #region StartPeriod

                if (!ChargingSchedulePeriodJSON.ParseMandatory("startPeriod",
                                                               "start period",
                                                               out UInt32  StartPeriod,
                                                               out String  ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Limit

                if (!ChargingSchedulePeriodJSON.ParseMandatory("limit",
                                                               "power limit",
                                                               out Decimal  Limit,
                                                               out          ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region NumberPhases

                if (ChargingSchedulePeriodJSON.ParseOptional("numberPhases",
                                                             "number of phases",
                                                             out Byte?  NumberPhases,
                                                             out        ErrorResponse))
                {

                    if (ErrorResponse != null)
                       return false;

                }

                #endregion


                ChargingSchedulePeriod = new ChargingSchedulePeriod(StartPeriod,
                                                                    Limit,
                                                                    NumberPhases);

                return true;

            }
            catch (Exception e)
            {

                OnException?.Invoke(DateTime.UtcNow, ChargingSchedulePeriodJSON, e);

                ChargingSchedulePeriod = default;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(ChargingSchedulePeriodText, out ChargingSchedulePeriod, OnException = null)

        /// <summary>
        /// Try to parse the given text representation of a charging schedule period.
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

                ChargingSchedulePeriodText = ChargingSchedulePeriodText?.Trim();

                if (ChargingSchedulePeriodText.IsNotNullOrEmpty())
                {

                    if (ChargingSchedulePeriodText.StartsWith("{") &&
                        TryParse(JObject.Parse(ChargingSchedulePeriodText),
                                 out ChargingSchedulePeriod,
                                 OnException))
                    {
                        return true;
                    }

                    if (TryParse(XDocument.Parse(ChargingSchedulePeriodText).Root,
                                 out ChargingSchedulePeriod,
                                 OnException))
                    {
                        return true;
                    }

                }

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

        #region ToJSON(CustomChargingSchedulePeriodSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingSchedulePeriod> CustomChargingSchedulePeriodSerializer = null)
        {

            var JSON = JSONObject.Create(

                           new JProperty("startPeriod",         StartPeriod.ToString()),
                           new JProperty("limit",               Limit.      ToString("0.#")),

                           NumberPhases.HasValue
                               ? new JProperty("numberPhases",  NumberPhases)
                               : null

                       );

            return CustomChargingSchedulePeriodSerializer != null
                       ? CustomChargingSchedulePeriodSerializer(this, JSON)
                       : JSON;

        }

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

            if (Object is null)
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

                   ((!NumberPhases.HasValue && !ChargingSchedulePeriod.NumberPhases.HasValue) ||
                     (NumberPhases.HasValue &&  ChargingSchedulePeriod.NumberPhases.HasValue && NumberPhases.Value.Equals(ChargingSchedulePeriod.NumberPhases.Value)));

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

                return StartPeriod.       GetHashCode() * 7 ^
                       Limit.             GetHashCode() * 5 ^

                       (NumberPhases.HasValue
                           ? NumberPhases.GetHashCode() * 3
                           : 0);

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
