/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of WWCP OCPP <https://github.com/OpenChargingCloud/WWCP_OCPP>
 *
 * Licensed under the Affero GPL license, Version 3.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.gnu.org/licenses/agpl.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

using cloud.charging.open.protocols.WWCP;
using cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonTypes;
using System;

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1.ISO15118_20.CommonMessages
{

    /// <summary>
    /// The price level schedule.
    /// </summary>
    public class PriceLevelSchedule : APriceSchedule,
                                      ICBORSerializable<PriceLevelSchedule>,
                                      IEquatable<PriceLevelSchedule>
    {

        #region Properties

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
        public PriceLevelSchedule(PriceSchedule_Id                      Id,
                                  DateTimeOffset                        TimeAnchor,
                                  Byte                                  NumberOfPriceLevels,
                                  IEnumerable<PriceLevelScheduleEntry>  PriceLevelScheduleEntries,
                                  String?                               Description   = null,
                                  CustomData?                           CustomData    = null)

            : base(Id,
                   TimeAnchor,
                   Description,
                   CustomData)

        {

            if (!PriceLevelScheduleEntries.Any())
                throw new ArgumentException("The given enumeration of price level schedule entries must not be empty!",
                                            nameof(PriceLevelScheduleEntries));

            this.NumberOfPriceLevels        = NumberOfPriceLevels;
            this.PriceLevelScheduleEntries  = PriceLevelScheduleEntries.Distinct();

            unchecked
            {

                hashCode = this.Id.                       GetHashCode()  * 7 ^
                           this.NumberOfPriceLevels.      GetHashCode()  * 5 ^
                           this.PriceLevelScheduleEntries.CalcHashCode() * 3 ^
                           base.                          GetHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "The PriceLevelScheduleType is modeled after the same type that is defined in ISO 15118-20, such that if it is supplied by an EMSP as a signed EXI message, the conversion from EXI to JSON (in OCPP) and back to EXI (for ISO 15118-20) does not change the digest and therefore does not invalidate the signature.",
        //     "javaType": "PriceLevelSchedule",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "priceLevelScheduleEntries": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/PriceLevelScheduleEntryType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 100
        //         },
        //         "timeAnchor": {
        //             "description": "Starting point of this price schedule.",
        //             "type": "string",
        //             "format": "date-time"
        //         },
        //         "priceScheduleId": {
        //             "description": "Unique ID of this price schedule.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "priceScheduleDescription": {
        //             "description": "Description of the price schedule.",
        //             "type": "string",
        //             "maxLength": 32
        //         },
        //         "numberOfPriceLevels": {
        //             "description": "Defines the overall number of distinct price level elements used across all PriceLevelSchedules.",
        //             "type": "integer",
        //             "minimum": 0.0
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     },
        //     "required": [
        //         "timeAnchor",
        //         "priceScheduleId",
        //         "numberOfPriceLevels",
        //         "priceLevelScheduleEntries"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomPriceLevelScheduleParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price level schedule.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="CustomPriceLevelScheduleParser">An optional delegate to parse custom price level schedules.</param>
        public static PriceLevelSchedule Parse(JObject                                           JSON,
                                               CustomJObjectParserDelegate<PriceLevelSchedule>?  CustomPriceLevelScheduleParser   = null)
        {

            if (TryParse(JSON,
                         out var priceLevelSchedule,
                         out var errorResponse,
                         CustomPriceLevelScheduleParser))
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
        public static Boolean TryParse(JObject                                       JSON,
                                       [NotNullWhen(true)]  out PriceLevelSchedule?  PriceLevelSchedule,
                                       [NotNullWhen(false)] out String?              ErrorResponse)

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
        /// <param name="CustomPriceLevelScheduleParser">An optional delegate to parse custom contract certificates.</param>
        public static Boolean TryParse(JObject                                           JSON,
                                       [NotNullWhen(true)]  out PriceLevelSchedule?      PriceLevelSchedule,
                                       [NotNullWhen(false)] out String?                  ErrorResponse,
                                       CustomJObjectParserDelegate<PriceLevelSchedule>?  CustomPriceLevelScheduleParser)
        {

            try
            {

                PriceLevelSchedule = null;

                #region Id                           [mandatory]

                if (!JSON.ParseMandatory("priceScheduleId",
                                         "price level schedule identification",
                                         PriceSchedule_Id.TryParse,
                                         out PriceSchedule_Id Id,
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

                // As it is written - it was read as "description".
                var Description = JSON.GetString("priceScheduleDescription");

                #endregion

                #region CustomData                   [optional]

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


                PriceLevelSchedule = new PriceLevelSchedule(
                                         Id,
                                         TimeAnchor,
                                         NumberOfPriceLevels,
                                         PriceLevelScheduleEntries,
                                         Description,
                                         CustomData
                                     );

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

                                 new JProperty("priceScheduleId",             Id.             Value),
                                 new JProperty("timeAnchor",                  TimeAnchor.     ToISO8601()),
                                 new JProperty("numberOfPriceLevels",         NumberOfPriceLevels),
                                 new JProperty("priceLevelScheduleEntries",   new JArray(PriceLevelScheduleEntries.Select(priceLevelScheduleEntry => priceLevelScheduleEntry.ToJSON(CustomPriceLevelScheduleEntrySerializer)))),

                           Description.IsNotNullOrEmpty()
                               ? new JProperty("priceScheduleDescription",    Description)
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",                  CustomData.ToJSON())
                               : null

                       );

            return CustomPriceLevelScheduleSerializer is not null
                       ? CustomPriceLevelScheduleSerializer(this, json)
                       : json;

        }

        #endregion


        #region (static) TryParseCBOR(CBOR, out PriceLevelSchedule, out ErrorResponse, CustomPriceLevelScheduleParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a price level schedule.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="PriceLevelSchedule">The price level schedule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                  CBOR,
                                       [NotNullWhen(true)]  out PriceLevelSchedule?  PriceLevelSchedule,
                                       [NotNullWhen(false)] out String?           ErrorResponse)

            => TryParseCBOR(CBOR,
                            out PriceLevelSchedule,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a price level schedule.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="PriceLevelSchedule">The price level schedule.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceLevelScheduleParser">An optional delegate to read custom price level schedules.</param>
        public static Boolean TryParseCBOR(CBORValue                                   CBOR,
                                       [NotNullWhen(true)]  out PriceLevelSchedule?           PriceLevelSchedule,
                                       [NotNullWhen(false)] out String?            ErrorResponse,
                                       CustomCBORParserDelegate<PriceLevelSchedule>?  CustomPriceLevelScheduleParser)
        {

            try
            {

                PriceLevelSchedule = default;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a price level schedule is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryUInt64("priceScheduleId",
                                               "price schedule identification",
                                               out var IdNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (IdNumber > UInt16.MaxValue || !PriceSchedule_Id.TryParse((UInt16) IdNumber, out var Id))
                {
                    ErrorResponse = $"Invalid price schedule identification '{IdNumber}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("timeAnchor",
                                              "time anchor",
                                              OCPPCBORExtensions.TryParseTimestamp,
                                              out DateTimeOffset TimeAnchor,
                                              out ErrorResponse))
                {
                    return false;
                }

                if (!CBOR.ParseMandatoryUInt64("numberOfPriceLevels",
                                               "number of price levels",
                                               out var NumberOfPriceLevelsNumber,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (NumberOfPriceLevelsNumber > Byte.MaxValue)
                {
                    ErrorResponse = $"Invalid number of price levels '{NumberOfPriceLevelsNumber}'!";
                    return false;
                }

                var NumberOfPriceLevels = (Byte) NumberOfPriceLevelsNumber;

                if (!CBOR.ParseMandatoryList<PriceLevelScheduleEntry>("priceLevelScheduleEntries",
                                                     "price level schedule entries",
                                                     PriceLevelScheduleEntry.TryParseCBOR,
                                                     out var PriceLevelScheduleEntries,
                                                     out ErrorResponse))
                {
                    return false;
                }

                CBOR.ParseOptionalText("priceScheduleDescription",
                                       "price schedule description",
                                       out var Description,
                                       out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;

                PriceLevelSchedule = new PriceLevelSchedule(
                                         Id,
                                         TimeAnchor,
                                         NumberOfPriceLevels,
                                         PriceLevelScheduleEntries,
                                         Description,
                                         CustomData
                                     );

                if (CustomPriceLevelScheduleParser is not null)
                    PriceLevelSchedule = CustomPriceLevelScheduleParser(CBOR,
                                                    PriceLevelSchedule);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                PriceLevelSchedule  = default;
                ErrorResponse  = "The given CBOR representation of a price level schedule is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<PriceLevelSchedule>.TryParse(CBOR, out PriceLevelSchedule, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a price level schedule - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<PriceLevelSchedule>.TryParse(CBORValue                         CBOR,
                                                                    out PriceLevelSchedule                  Value,
                                                                    [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomPriceLevelScheduleSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this price level schedule: the keys of
        /// its JSON object, and its values as what they are.
        /// </summary>
        /// <param name="CustomPriceLevelScheduleSerializer">A delegate to serialize custom price level schedules.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<PriceLevelSchedule>? CustomPriceLevelScheduleSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("priceScheduleId",            CBORValue.FromUInt64(Id.Value)),
                           ("timeAnchor",                 TimeAnchor.ToCBOR()),
                           ("numberOfPriceLevels",        CBORValue.FromUInt64(NumberOfPriceLevels)),
                           ("priceLevelScheduleEntries",  CBORValue.FromArray(PriceLevelScheduleEntries.Select(priceLevelScheduleEntry => priceLevelScheduleEntry.ToCBOR()))),
                           ("priceScheduleDescription",   OCPPCBORExtensions.Text(Description)),
                           ("customData",              CustomData?.ToCBOR())
                       );

            return CustomPriceLevelScheduleSerializer is not null
                       ? CustomPriceLevelScheduleSerializer(this, cbor)
                       : cbor;

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

            => $"{Id}, {NumberOfPriceLevels} price level(s), {PriceLevelScheduleEntries.Count()} price level schedule entry/entries";

        #endregion

    }

}
