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

using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

using cloud.charging.open.protocols.WWCP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A charging period.
    /// </summary>
    public class ChargingPeriod : ACustomData,
                                  IEquatable<ChargingPeriod>,
                                  IComparable<ChargingPeriod>,
                                  IComparable
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this data structure.
        /// </summary>
        public static readonly JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://open.charging.cloud/context/ocpp/v2.1/cdr");

        #endregion

        #region Properties

        /// <summary>
        /// The starting time of the charging period relative to start of the charging session.
        /// </summary>
        [Mandatory]
        public TimeSpan                          StartPeriod      { get; }

        /// <summary>
        /// The optional unique charging tariff identification used to calculate the costs for this charging period.
        /// </summary>
        [Optional]
        public Tariff_Id?                ChargingTariffId    { get; }

        /// <summary>
        /// The optional enumeration of volume per cost dimension for this charging period.
        /// </summary>
        [Optional]
        public IEnumerable<CostDimensionVolume>  Costs               { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging period.
        /// </summary>
        /// <param name="StartPeriod">The starting time of the charging period relative to start of the charging session.</param>
        /// <param name="ChargingTariffId">An optional unique charging tariff identification used to calculate the costs for this charging period.</param>
        /// <param name="Costs">An optional enumeration of volume per cost dimension for this charging period.</param>
        /// 
        /// <param name="CustomData">An optional custom data object to allow to store any kind of customer specific data.</param>
        public ChargingPeriod(TimeSpan                           StartPeriod,
                              Tariff_Id?                 ChargingTariffId   = null,
                              IEnumerable<CostDimensionVolume>?  Costs              = null,
                              CustomData?                        CustomData         = null)

            : base(CustomData)

        {

            this.StartPeriod       = StartPeriod;
            this.ChargingTariffId  = ChargingTariffId;
            this.Costs             = Costs?.Distinct() ?? Array.Empty<CostDimensionVolume>();


            unchecked
            {

                hashCode = this.StartPeriod.      GetHashCode()       * 7 ^
                          (this.ChargingTariffId?.GetHashCode() ?? 0) * 5 ^
                           this.Costs.            CalcHashCode()      * 3 ^
                           base.                  GetHashCode();

            }

        }

        #endregion


        #region (static) Parse   (JSON, CustomChargingPeriodParser = null)

        /// <summary>
        /// Parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomChargingPeriodParser">A delegate to parse custom charging period JSON objects.</param>
        public static ChargingPeriod Parse(JObject                                       JSON,
                                           CustomJObjectParserDelegate<ChargingPeriod>?  CustomChargingPeriodParser   = null)
        {

            if (TryParse(JSON,
                         out var chargingPeriod,
                         out var errorResponse,
                         CustomChargingPeriodParser) &&
                chargingPeriod is not null)
            {
                return chargingPeriod;
            }

            throw new ArgumentException("The given JSON representation of a charging period is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out ChargingPeriod, out ErrorResponse, TariffIdURL = null, CustomTariffParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingPeriod">The parsed charging period.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject              JSON,
                                       out ChargingPeriod?  ChargingPeriod,
                                       out String?          ErrorResponse)

            => TryParse(JSON,
                        out ChargingPeriod,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="ChargingPeriod">The parsed charging period.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingPeriodParser">A delegate to parse custom charging period JSON objects.</param>
        public static Boolean TryParse(JObject                                       JSON,
                                       out ChargingPeriod?                           ChargingPeriod,
                                       out String?                                   ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingPeriod>?  CustomChargingPeriodParser   = null)
        {

            try
            {

                ChargingPeriod = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse StartPeriod         [mandatory]

                if (!JSON.ParseMandatory("startPeriod",
                                         "start of the charging period",
                                         out TimeSpan StartPeriod,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ChargingTariffId    [optional]

                if (JSON.ParseOptional("tariffId",
                                       "tariff identification",
                                       Tariff_Id.TryParse,
                                       out Tariff_Id? ChargingTariffId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse Costs               [mandatory]

                if (JSON.ParseOptionalHashSet("costs",
                                              "charging costs",
                                              CostDimensionVolume.TryParse,
                                              out HashSet<CostDimensionVolume> Costs,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                #region CustomData                [optional]

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


                ChargingPeriod = new ChargingPeriod(

                                     StartPeriod,
                                     ChargingTariffId,
                                     Costs,

                                     CustomData

                                 );

                if (CustomChargingPeriodParser is not null)
                    ChargingPeriod = CustomChargingPeriodParser(JSON,
                                                                ChargingPeriod);

                return true;

            }
            catch (Exception e)
            {
                ChargingPeriod  = default;
                ErrorResponse   = "The given JSON representation of a charging period is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffSerializer = null, CustomCostDimensionValueSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffSerializer">A delegate to serialize custom tariff JSON objects.</param>
        /// <param name="CustomCostDimensionValueSerializer">A delegate to serialize custom cost dimension volume JSON objects.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingPeriod>?       CustomTariffSerializer               = null,
                              CustomJObjectSerializerDelegate<CostDimensionVolume>?  CustomCostDimensionValueSerializer   = null,
                              CustomJObjectSerializerDelegate<CustomData>?           CustomCustomDataSerializer           = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("startPeriod",          StartPeriod.     TotalSeconds),

                           ChargingTariffId.HasValue
                               ? new JProperty("chargingPeriodId",     ChargingTariffId.ToString())
                               : null,

                           Costs.           Any()
                               ? new JProperty("costs",                new JArray(Costs.Select(costDimensionVolume  => costDimensionVolume.ToJSON(CustomCostDimensionValueSerializer))))
                               : null,


                           CustomData is not null
                               ? new JProperty("customData",           CustomData.      ToJSON(CustomCustomDataSerializer))
                               : null);

            return CustomTariffSerializer is not null
                       ? CustomTariffSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this charging period.
        /// </summary>
        public ChargingPeriod Clone()

            => new (

                   StartPeriod,
                   ChargingTariffId?.Clone,
                   Costs.Select(costDimensionVolume => costDimensionVolume.Clone()).ToArray(),

                   CustomData

               );

        #endregion


        #region Operator overloading

        #region Operator == (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ChargingPeriod? ChargingPeriod1,
                                           ChargingPeriod? ChargingPeriod2)
        {

            if (Object.ReferenceEquals(ChargingPeriod1, ChargingPeriod2))
                return true;

            if (ChargingPeriod1 is null || ChargingPeriod2 is null)
                return false;

            return ChargingPeriod1.Equals(ChargingPeriod2);

        }

        #endregion

        #region Operator != (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ChargingPeriod? ChargingPeriod1,
                                           ChargingPeriod? ChargingPeriod2)

            => !(ChargingPeriod1 == ChargingPeriod2);

        #endregion

        #region Operator <  (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ChargingPeriod? ChargingPeriod1,
                                          ChargingPeriod? ChargingPeriod2)

            => ChargingPeriod1 is null
                   ? throw new ArgumentNullException(nameof(ChargingPeriod1), "The given charging period must not be null!")
                   : ChargingPeriod1.CompareTo(ChargingPeriod2) < 0;

        #endregion

        #region Operator <= (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ChargingPeriod? ChargingPeriod1,
                                           ChargingPeriod? ChargingPeriod2)

            => !(ChargingPeriod1 > ChargingPeriod2);

        #endregion

        #region Operator >  (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ChargingPeriod? ChargingPeriod1,
                                          ChargingPeriod? ChargingPeriod2)

            => ChargingPeriod1 is null
                   ? throw new ArgumentNullException(nameof(ChargingPeriod1), "The given charging period must not be null!")
                   : ChargingPeriod1.CompareTo(ChargingPeriod2) > 0;

        #endregion

        #region Operator >= (ChargingPeriod1, ChargingPeriod2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ChargingPeriod1">A charging period.</param>
        /// <param name="ChargingPeriod2">Another charging period.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ChargingPeriod? ChargingPeriod1,
                                           ChargingPeriod? ChargingPeriod2)

            => !(ChargingPeriod1 < ChargingPeriod2);

        #endregion

        #endregion

        #region IComparable<ChargingPeriod> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two charging periods.
        /// </summary>
        /// <param name="Object">A charging period to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is ChargingPeriod chargingPeriod
                   ? CompareTo(chargingPeriod)
                   : throw new ArgumentException("The given object is not a charging period!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ChargingPeriod)

        /// <summary>
        /// Compares two charging periods.
        /// </summary>
        /// <param name="ChargingPeriod">A charging period to compare with.</param>
        public Int32 CompareTo(ChargingPeriod? ChargingPeriod)
        {

            if (ChargingPeriod is null)
                throw new ArgumentNullException(nameof(ChargingPeriod), "The given charging period must not be null!");

            var c = StartPeriod.CompareTo(ChargingPeriod.StartPeriod);

            if (c == 0 && ChargingTariffId.HasValue && ChargingPeriod.ChargingTariffId.HasValue)
                c = ChargingTariffId.Value.CompareTo(ChargingPeriod.ChargingTariffId.Value);

            // Costs

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<ChargingPeriod> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="Object">A charging period to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingPeriod chargingPeriod &&
                   Equals(chargingPeriod);

        #endregion

        #region Equals(ChargingPeriod)

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="ChargingPeriod">A charging period to compare with.</param>
        public Boolean Equals(ChargingPeriod? ChargingPeriod)

            => ChargingPeriod is not null &&

               StartPeriod.Equals(ChargingPeriod.StartPeriod) &&

            ((!ChargingTariffId.HasValue && !ChargingPeriod.ChargingTariffId.HasValue) ||
              (ChargingTariffId.HasValue &&  ChargingPeriod.ChargingTariffId.HasValue && ChargingTariffId.Value.Equals(ChargingPeriod.ChargingTariffId.Value))) &&

               Costs.Count().Equals(ChargingPeriod.Costs.Count()) &&
               Costs.All(ChargingPeriod.Costs.Contains);

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

            => String.Concat(

                   StartPeriod,

                   ChargingTariffId.HasValue
                       ? $" ({ChargingTariffId})"
                       : "",

                   Costs.Any()
                       ? ", " + Costs.Select(costDimensionValue => costDimensionValue.ToString()).AggregateWith(", ")
                       : ""

               );

        #endregion


    }

}
