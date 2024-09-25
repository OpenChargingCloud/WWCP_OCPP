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
    /// A charging schedule period.
    /// </summary>
    public readonly struct ChargingSchedulePeriod : IEquatable<ChargingSchedulePeriod>
    {

        #region Properties

        /// <summary>
        /// The start of the period relative to the start of the charging schedule.
        /// This value also defines the stop time of the previous period.
        /// </summary>
        [Mandatory]
        public TimeSpan  StartPeriod     { get; }

        /// <summary>
        /// Power limit during the schedule period in Amperes.
        /// </summary>
        [Mandatory]
        public Decimal   Limit           { get; }

        /// <summary>
        /// The number of phases that can be used for charging.
        /// </summary>
        [Optional]
        public Byte?     NumberPhases    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging schedule period.
        /// </summary>
        /// <param name="StartPeriod">The start of the period relative to the start of the charging schedule. This value also defines the stop time of the previous period.</param>
        /// <param name="Limit">Power limit during the schedule period in Amperes.</param>
        /// <param name="NumberPhases">The number of phases that can be used for charging.</param>
        public ChargingSchedulePeriod(TimeSpan  StartPeriod,
                                      Decimal   Limit,
                                      Byte?     NumberPhases   = null)
        {

            this.StartPeriod   = StartPeriod;
            this.Limit         = Limit;
            this.NumberPhases  = NumberPhases;

            unchecked
            {

                hashCode = this.StartPeriod.  GetHashCode() * 5 ^
                           this.Limit.        GetHashCode() * 3 ^
                          (this.NumberPhases?.GetHashCode() ?? 0);

            }

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

        #region (static) Parse   (XML)

        /// <summary>
        /// Parse the given XML representation of a charging schedule period.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        public static ChargingSchedulePeriod Parse(XElement XML)
        {

            if (TryParse(XML,
                         out var chargingSchedulePeriod,
                         out var errorResponse))
            {
                return chargingSchedulePeriod;
            }

            throw new ArgumentException("The given XML representation of a charging schedule period is invalid: " + errorResponse,
                                        nameof(XML));

        }

        #endregion

        #region (static) Parse   (JSON, CustomChargingSchedulePeriodParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging schedule period.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomChargingSchedulePeriodParser">An optional delegate to parse custom CustomChargingSchedulePeriod JSON objects.</param>
        public static ChargingSchedulePeriod Parse(JObject                                               JSON,
                                                   CustomJObjectParserDelegate<ChargingSchedulePeriod>?  CustomChargingSchedulePeriodParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingSchedulePeriod,
                         out var errorResponse,
                         CustomChargingSchedulePeriodParser))
            {
                return chargingSchedulePeriod;
            }

            throw new ArgumentException("The given JSON representation of a charging schedule period is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(XML,  out ChargingSchedulePeriod, out ErrorResponse)

        /// <summary>
        /// Try to parse the given XML representation of a charging schedule period.
        /// </summary>
        /// <param name="XML">The XML to be parsed.</param>
        /// <param name="ChargingSchedulePeriod">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(XElement                                         XML,
                                       [NotNullWhen(true)]  out ChargingSchedulePeriod  ChargingSchedulePeriod,
                                       [NotNullWhen(false)] out String?                 ErrorResponse)
        {

            ErrorResponse = null;

            try
            {

                ChargingSchedulePeriod = new ChargingSchedulePeriod(

                                             TimeSpan.FromSeconds(XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "startPeriod",
                                                                                         UInt32.Parse)),

                                             XML.MapValueOrFail    (OCPPNS.OCPPv1_6_CP + "limit",
                                                                    Decimal.Parse),

                                             XML.MapValueOrNullable(OCPPNS.OCPPv1_6_CP + "numberPhases",
                                                                    Byte.Parse)

                                         );

                return true;

            }
            catch (Exception e)
            {
                ChargingSchedulePeriod  = default;
                ErrorResponse           = "The given JSON representation of a charging schedule period is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingSchedulePeriod, out ErrorResponse, CustomChargingSchedulePeriodParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging schedule period.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingSchedulePeriod">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                          JSON,
                                       [NotNullWhen(true)]  out ChargingSchedulePeriod  ChargingSchedulePeriod,
                                       [NotNullWhen(false)] out String?                 ErrorResponse)

            => TryParse(JSON,
                        out ChargingSchedulePeriod,
                        out ErrorResponse,
                        null);



        /// <summary>
        /// Try to parse the given JSON representation of a charging schedule period.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingSchedulePeriod">The parsed connector type.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingSchedulePeriodParser">An optional delegate to parse custom CustomChargingSchedulePeriod JSON objects.</param>
        public static Boolean TryParse(JObject                                               JSON,
                                       [NotNullWhen(true)]  out ChargingSchedulePeriod       ChargingSchedulePeriod,
                                       [NotNullWhen(false)] out String?                      ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingSchedulePeriod>?  CustomChargingSchedulePeriodParser)
        {

            try
            {

                ChargingSchedulePeriod = default;

                #region StartPeriod     [mandatory]

                if (!JSON.ParseMandatory("startPeriod",
                                         "start period",
                                         out TimeSpan StartPeriod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Limit           [mandatory]

                if (!JSON.ParseMandatory("limit",
                                         "power limit",
                                         out Decimal Limit,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region NumberPhases    [optional]

                if (JSON.ParseOptional("numberPhases",
                                       "number of phases",
                                       out Byte? NumberPhases,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                       return false;
                }

                #endregion


                ChargingSchedulePeriod = new ChargingSchedulePeriod(
                                             StartPeriod,
                                             Limit,
                                             NumberPhases
                                         );


                if (CustomChargingSchedulePeriodParser is not null)
                    ChargingSchedulePeriod = CustomChargingSchedulePeriodParser(JSON,
                                                                                ChargingSchedulePeriod);

                return true;

            }
            catch (Exception e)
            {
                ChargingSchedulePeriod  = default;
                ErrorResponse           = "The given JSON representation of a charging schedule period is invalid: " + e.Message;
                return false;

            }

        }

        #endregion

        #region ToXML (XName = null)

        /// <summary>
        /// Return a XML representation of this object.
        /// </summary>
        /// <param name="XName">An alternative XML element name [default: "OCPPv1_6_CP:chargingSchedulePeriod"]</param>
        public XElement ToXML(XName? XName = null)

            => new (XName ?? OCPPNS.OCPPv1_6_CP + "chargingSchedulePeriod",

                   new XElement(OCPPNS.OCPPv1_6_CP + "startPeriod",   (UInt32) Math.Round(StartPeriod.TotalSeconds, 0)),
                   new XElement(OCPPNS.OCPPv1_6_CP + "limit",         Limit.ToString("0.#")),
                   new XElement(OCPPNS.OCPPv1_6_CP + "numberPhases",  NumberPhases)

               );

        #endregion

        #region ToJSON(CustomChargingSchedulePeriodSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomChargingSchedulePeriodSerializer">A delegate to serialize custom charging schedule periods.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingSchedulePeriod>? CustomChargingSchedulePeriodSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("startPeriod",    (UInt32) Math.Round(StartPeriod.TotalSeconds, 0)),
                                 new JProperty("limit",          Math.Round(Limit, 1)),

                           NumberPhases.HasValue
                               ? new JProperty("numberPhases",   NumberPhases)
                               : null

                       );

            return CustomChargingSchedulePeriodSerializer is not null
                       ? CustomChargingSchedulePeriodSerializer(this, json)
                       : json;

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
        public static Boolean operator == (ChargingSchedulePeriod ChargingSchedulePeriod1,
                                           ChargingSchedulePeriod ChargingSchedulePeriod2)

            => ChargingSchedulePeriod1.Equals(ChargingSchedulePeriod2);

        #endregion

        #region Operator != (ChargingSchedulePeriod1, ChargingSchedulePeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingSchedulePeriod1">An id tag info.</param>
        /// <param name="ChargingSchedulePeriod2">Another id tag info.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingSchedulePeriod ChargingSchedulePeriod1,
                                           ChargingSchedulePeriod ChargingSchedulePeriod2)

            => !ChargingSchedulePeriod1.Equals(ChargingSchedulePeriod2);

        #endregion

        #endregion

        #region IEquatable<ChargingSchedulePeriod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging schedule periods for equality..
        /// </summary>
        /// <param name="Object">A charging schedule period to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingSchedulePeriod chargingSchedulePeriod &&
                   Equals(chargingSchedulePeriod);

        #endregion

        #region Equals(ChargingSchedulePeriod)

        /// <summary>
        /// Compares two charging schedule periods for equality.
        /// </summary>
        /// <param name="ChargingSchedulePeriod">A charging schedule period to compare with.</param>
        public Boolean Equals(ChargingSchedulePeriod ChargingSchedulePeriod)

            => StartPeriod. Equals(ChargingSchedulePeriod.StartPeriod) &&
               Limit.       Equals(ChargingSchedulePeriod.Limit)       &&

            ((!NumberPhases.HasValue && !ChargingSchedulePeriod.NumberPhases.HasValue) ||
              (NumberPhases.HasValue &&  ChargingSchedulePeriod.NumberPhases.HasValue && NumberPhases.Value.Equals(ChargingSchedulePeriod.NumberPhases.Value)));

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

            => String.Concat(StartPeriod,
                             " / ",
                             " with ", Limit, " Ampere",

                             NumberPhases.HasValue
                                 ? ", " + NumberPhases + " phases"
                                 : "");

        #endregion

    }

}
