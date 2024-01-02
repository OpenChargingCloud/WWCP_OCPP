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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonTypes;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The price level schedule.
    /// </summary>
    public class PriceLevelSchedule : PriceSchedule,
                                      IEquatable<PriceLevelSchedule>
    {

        //ToDo: Ask OCA about PriceLevelSchedule_Id which seems to be a XML attribute in 15118-20, but missing in OCPP v2.1!
        //ToDo: OCPP is using the undefined type "NumericIdentifierType"!

        #region Properties

        /// <summary>
        /// The unique identification of the price level schedule.
        /// </summary>
        [Mandatory]
        public PriceLevelSchedule_Id                 Id                           { get; }

        /// <summary>
        /// The the overall number of distinct price level elements used across
        /// all price level schedules.
        /// </summary>
        [Mandatory]
        public Byte                                  NumberOfPriceLevels          { get; }

        /// <summary>
        /// The enumeration of price level schedule entries.
        /// </summary>
        [Mandatory]
        public IEnumerable<PriceLevelScheduleEntry>  PriceLevelScheduleEntries    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price level schedule.
        /// </summary>
        /// <param name="Id">An unique identification for the price level schedule.</param>
        /// <param name="TimeAnchor">A time anchor of the price schedule.</param>
        /// <param name="PriceScheduleId">An unique identification of the price schedule.</param>
        /// <param name="NumberOfPriceLevels">The number of prive levels.</param>
        /// <param name="PriceLevelScheduleEntries">An enumeration of price level schedule entries (max 1024).</param>
        /// <param name="Description">An optional description of the price schedule.</param>
        public PriceLevelSchedule(PriceLevelSchedule_Id                 Id,
                                  DateTime                              TimeAnchor,
                                  PriceSchedule_Id                      PriceScheduleId,
                                  Byte                                  NumberOfPriceLevels,
                                  IEnumerable<PriceLevelScheduleEntry>  PriceLevelScheduleEntries,
                                  Description?                          Description   = null)

            : base(TimeAnchor,
                   PriceScheduleId,
                   Description)

        {

            if (!PriceLevelScheduleEntries.Any())
                throw new ArgumentException("The given enumeration of price level schedule entries must not be empty!",
                                            nameof(PriceLevelScheduleEntries));

            this.Id                         = Id;
            this.NumberOfPriceLevels        = NumberOfPriceLevels;
            this.PriceLevelScheduleEntries  = PriceLevelScheduleEntries.Distinct();

        }

        #endregion


        //ToDo: Update schema documentation after the official release of OCPP v2.1!

        #region Documentation


        #endregion

        #region (static) Parse   (JSON, CustomPriceLevelScheduleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price level schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPriceLevelScheduleParser">A delegate to parse custom price level schedules.</param>
        public static PriceLevelSchedule Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<PriceLevelSchedule>?  CustomPriceLevelScheduleParser   = null)
        {

            if (TryParse(JSON,
                         out var priceLevelSchedule,
                         out var errorResponse,
                         CustomPriceLevelScheduleParser) &&
                priceLevelSchedule is not null)
            {
                return priceLevelSchedule;
            }

            throw new ArgumentException("The given JSON representation of a price level schedule is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out PriceLevelSchedule, out ErrorResponse, CustomPriceLevelScheduleParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a price level schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PriceLevelSchedule">The parsed price level schedule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                  JSON,
                                       out PriceLevelSchedule?  PriceLevelSchedule,
                                       out String?              ErrorResponse)

            => TryParse(JSON,
                        out PriceLevelSchedule,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a price level schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="PriceLevelSchedule">The parsed price level schedule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceLevelScheduleParser">A delegate to parse custom contract certificates.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       out PriceLevelSchedule?                           PriceLevelSchedule,
                                       out String?                                       ErrorResponse,
                                       CustomJObjectParserDelegate<PriceLevelSchedule>?  CustomPriceLevelScheduleParser)
        {

            try
            {

                PriceLevelSchedule = null;

                #region Id                           [mandatory]

                if (!JSON.ParseMandatory("id",
                                         "price level schedule identification",
                                         PriceLevelSchedule_Id.TryParse,
                                         out PriceLevelSchedule_Id Id,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region TimeAnchor                   [mandatory]

                if (!JSON.ParseMandatory("timeAnchor",
                                         "time anchor",
                                         out DateTime TimeAnchor,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PriceScheduleId              [mandatory]

                if (!JSON.ParseMandatory("priceScheduleId",
                                         "price schedule identification",
                                         PriceSchedule_Id.TryParse,
                                         out PriceSchedule_Id PriceScheduleId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region NumberOfPriceLevels          [mandatory]

                if (!JSON.ParseMandatory("numberOfPriceLevels",
                                         "number of price levels",
                                         out Byte NumberOfPriceLevels,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region PriceLevelScheduleEntries    [mandatory]

                if (!JSON.ParseMandatoryHashSet("priceLevelScheduleEntries",
                                                "price level schedule entries",
                                                PriceLevelScheduleEntry.TryParse,
                                                out HashSet<PriceLevelScheduleEntry> PriceLevelScheduleEntries,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Description                  [optional]

                if (JSON.ParseOptional("description",
                                       "price schedule description",
                                       CommonTypes.Description.TryParse,
                                       out Description? Description,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                PriceLevelSchedule = new PriceLevelSchedule(Id,
                                                            TimeAnchor,
                                                            PriceScheduleId,
                                                            NumberOfPriceLevels,
                                                            PriceLevelScheduleEntries,
                                                            Description);

                if (CustomPriceLevelScheduleParser is not null)
                    PriceLevelSchedule = CustomPriceLevelScheduleParser(JSON,
                                                                        PriceLevelSchedule);

                return true;

            }
            catch (Exception e)
            {
                PriceLevelSchedule  = null;
                ErrorResponse       = "The given JSON representation of a price level schedule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPriceLevelScheduleSerializer = null, CustomPriceLevelScheduleEntrySerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPriceLevelScheduleSerializer">A delegate to serialize custom price level schedules.</param>
        /// <param name="CustomPriceLevelScheduleEntrySerializer">A delegate to serialize custom price level schedule entries.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<PriceLevelSchedule>?       CustomPriceLevelScheduleSerializer        = null,
                              CustomJObjectSerializerDelegate<PriceLevelScheduleEntry>?  CustomPriceLevelScheduleEntrySerializer   = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("id",                         Id.             ToString()),
                                 new JProperty("timeAnchor",                 TimeAnchor.     ToIso8601()),
                                 new JProperty("priceScheduleId",            PriceScheduleId.ToString()),
                                 new JProperty("numberOfPriceLevels",        NumberOfPriceLevels),
                                 new JProperty("priceLevelScheduleEntries",  new JArray(PriceLevelScheduleEntries.Select(priceLevelScheduleEntry => priceLevelScheduleEntry.ToJSON(CustomPriceLevelScheduleEntrySerializer)))),

                           Description.HasValue
                               ? new JProperty("description",                Description.    Value)
                               : null

                       );

            return CustomPriceLevelScheduleSerializer is not null
                       ? CustomPriceLevelScheduleSerializer(this, json)
                       : json;

        }

        #endregion


        #region Operator overloading

        #region Operator == (PriceLevelSchedule1, PriceLevelSchedule2)

        /// <summary>
        /// Compares two price level schedules for equality.
        /// </summary>
        /// <param name="PriceLevelSchedule1">A price level schedule.</param>
        /// <param name="PriceLevelSchedule2">Another price level schedule.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public static Boolean operator == (PriceLevelSchedule? PriceLevelSchedule1,
                                           PriceLevelSchedule? PriceLevelSchedule2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(PriceLevelSchedule1, PriceLevelSchedule2))
                return true;

            // If one is null, but not both, return false.
            if (PriceLevelSchedule1 is null || PriceLevelSchedule2 is null)
                return false;

            return PriceLevelSchedule1.Equals(PriceLevelSchedule2);

        }

        #endregion

        #region Operator != (PriceLevelSchedule1, PriceLevelSchedule2)

        /// <summary>
        /// Compares two price level schedules for inequality.
        /// </summary>
        /// <param name="PriceLevelSchedule1">A price level schedule.</param>
        /// <param name="PriceLevelSchedule2">Another price level schedule.</param>
        /// <returns>False if both match; True otherwise.</returns>
        public static Boolean operator != (PriceLevelSchedule? PriceLevelSchedule1,
                                           PriceLevelSchedule? PriceLevelSchedule2)

            => !(PriceLevelSchedule1 == PriceLevelSchedule2);

        #endregion

        #endregion

        #region IEquatable<PriceLevelSchedule> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two price level schedules for equality.
        /// </summary>
        /// <param name="Object">A price level schedule to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is PriceLevelSchedule priceLevelSchedule &&
                   Equals(priceLevelSchedule);

        #endregion

        #region Equals(PriceLevelSchedule)

        /// <summary>
        /// Compares two price level schedules for equality.
        /// </summary>
        /// <param name="PriceLevelSchedule">A price level schedule to compare with.</param>
        public Boolean Equals(PriceLevelSchedule? PriceLevelSchedule)

            => PriceLevelSchedule is not null &&

               Id.                 Equals(PriceLevelSchedule.Id)                  &&
               NumberOfPriceLevels.Equals(PriceLevelSchedule.NumberOfPriceLevels) &&

               PriceLevelScheduleEntries.Count().Equals(PriceLevelSchedule.PriceLevelScheduleEntries.Count()) &&
               PriceLevelScheduleEntries.All(priceLevelScheduleEntry => PriceLevelSchedule.PriceLevelScheduleEntries.Contains(priceLevelScheduleEntry)) &&

               base.               Equals(PriceLevelSchedule);

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

                return Id.                       GetHashCode()  * 7 ^
                       NumberOfPriceLevels.      GetHashCode()  * 5 ^
                       PriceLevelScheduleEntries.CalcHashCode() * 3 ^

                       base.                     GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => $"{Id}, {NumberOfPriceLevels} price level(s), {PriceLevelScheduleEntries.Count()} price level schedule entry/entries";

        #endregion

    }

}
