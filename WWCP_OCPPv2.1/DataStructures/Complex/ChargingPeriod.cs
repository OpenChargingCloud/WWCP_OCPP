/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using cloud.charging.open.protocols.OCPP;

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A charging period of the cost details of a transaction: its start, and
    /// the volumes of what influenced its cost - the energy charged, the
    /// maximum current, ...
    /// </summary>
    public class ChargingPeriod : IEquatable<ChargingPeriod>,
                                  ICBORSerializable<ChargingPeriod>
    {

        #region Properties

        /// <summary>
        /// The start of the charging period. A period ends when the next period
        /// starts, the last period when the session ends.
        /// </summary>
        [Mandatory]
        public DateTimeOffset              StartPeriod    { get; }

        /// <summary>
        /// The optional identification of the tariff the cost was calculated with.
        /// </summary>
        [Optional]
        public Tariff_Id?                  TariffId       { get; }

        /// <summary>
        /// The volumes of the dimensions of the cost of this period.
        /// </summary>
        [Optional]
        public IEnumerable<CostDimension>  Dimensions     { get; }

        /// <summary>
        /// An optional custom data object allowing to store any kind of customer specific data.
        /// </summary>
        [Optional]
        public CustomData?                 CustomData     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new charging period.
        /// </summary>
        /// <param name="StartPeriod">The start of the charging period.</param>
        /// <param name="Dimensions">The volumes of the dimensions of the cost of this period.</param>
        /// <param name="TariffId">The optional identification of the tariff the cost was calculated with.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public ChargingPeriod(DateTimeOffset               StartPeriod,
                              IEnumerable<CostDimension>?  Dimensions   = null,
                              Tariff_Id?                   TariffId     = null,
                              CustomData?                  CustomData   = null)
        {

            this.StartPeriod  = StartPeriod;
            this.Dimensions   = Dimensions?.ToArray() ?? [];
            this.TariffId     = TariffId;
            this.CustomData   = CustomData;

            unchecked
            {
                hashCode = this.StartPeriod.GetHashCode()       * 7 ^
                           this.Dimensions. CalcHashCode()      * 5 ^
                          (this.TariffId?.  GetHashCode() ?? 0) * 3 ^
                          (this.CustomData?.GetHashCode() ?? 0);
            }

        }

        #endregion


        #region Documentation

        // "ChargingPeriodType": {
        //   "javaType": "ChargingPeriod",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "dimensions":  { "type": "array", "additionalItems": false, "items": { "$ref": "#/definitions/CostDimensionType" }, "minItems": 1 },
        //     "tariffId":    { "description": "Unique identifier of the Tariff that was used to calculate cost. If not provided, then cost was calculated by some other means.", "type": "string", "maxLength": 60 },
        //     "startPeriod": { "description": "Start timestamp of charging period. A period ends when the next period starts. The last period ends when the session ends.", "type": "string", "format": "date-time" },
        //     "customData":  { "$ref": "#/definitions/CustomDataType" }
        //   },
        //   "required": [ "startPeriod" ]
        // }

        #endregion

        #region (static) TryParse    (JSON, out ChargingPeriod, out ErrorResponse, CustomChargingPeriodParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingPeriod">The parsed charging period.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out ChargingPeriod?       ChargingPeriod,
                                       [NotNullWhen(false)] out String?               ErrorResponse)

            => TryParse(JSON,
                        out ChargingPeriod,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a charging period.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="ChargingPeriod">The parsed charging period.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingPeriodParser">An optional delegate to parse custom charging periods.</param>
        public static Boolean TryParse(JObject                                        JSON,
                                       [NotNullWhen(true)]  out ChargingPeriod?       ChargingPeriod,
                                       [NotNullWhen(false)] out String?               ErrorResponse,
                                       CustomJObjectParserDelegate<ChargingPeriod>?   CustomChargingPeriodParser)
        {

            try
            {

                ChargingPeriod = null;

                if (!JSON.ParseMandatory("startPeriod",
                                         "start of the charging period",
                                         out DateTimeOffset StartPeriod,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (JSON.ParseOptional("tariffId",
                                       "tariff identification",
                                       Tariff_Id.TryParse,
                                       out Tariff_Id? TariffId,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (JSON.ParseOptionalHashSet("dimensions",
                                              "cost dimensions",
                                              CostDimension.TryParse,
                                              out HashSet<CostDimension> Dimensions,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (JSON.ParseOptionalJSON("customData",
                                           "custom data",
                                           WWCP.CustomData.TryParse,
                                           out CustomData? CustomData,
                                           out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }


                ChargingPeriod = new ChargingPeriod(
                                     StartPeriod,
                                     Dimensions,
                                     TariffId,
                                     CustomData
                                 );

                if (CustomChargingPeriodParser is not null)
                    ChargingPeriod = CustomChargingPeriodParser(JSON,
                                                                ChargingPeriod);

                return true;

            }
            catch (Exception e)
            {
                ChargingPeriod  = null;
                ErrorResponse   = "The given JSON representation of a charging period is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomChargingPeriodSerializer = null, CustomCostDimensionSerializer = null, CustomCustomDataSerializer = null)

        /// <summary>
        /// Return a JSON representation of this charging period.
        /// </summary>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging periods.</param>
        /// <param name="CustomCostDimensionSerializer">A delegate to serialize custom volumes of cost dimensions.</param>
        /// <param name="CustomCustomDataSerializer">A delegate to serialize CustomData objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<ChargingPeriod>?  CustomChargingPeriodSerializer   = null,
                              CustomJObjectSerializerDelegate<CostDimension>?   CustomCostDimensionSerializer    = null,
                              CustomJObjectSerializerDelegate<CustomData>?      CustomCustomDataSerializer       = null)
        {

            var json = JSONObject.Create(

                           Dimensions.Any()
                               ? new JProperty("dimensions",    new JArray(Dimensions.Select(dimension => dimension.ToJSON(CustomCostDimensionSerializer,
                                                                                                                           CustomCustomDataSerializer))))
                               : null,

                           TariffId.HasValue
                               ? new JProperty("tariffId",      TariffId.Value.ToString())
                               : null,

                                 new JProperty("startPeriod",   StartPeriod.ToISO8601()),

                           CustomData is not null
                               ? new JProperty("customData",    CustomData.ToJSON(CustomCustomDataSerializer))
                               : null

                       );

            return CustomChargingPeriodSerializer is not null
                       ? CustomChargingPeriodSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, out ChargingPeriod, out ErrorResponse, CustomChargingPeriodParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a charging period.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="ChargingPeriod">The charging period.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                     CBOR,
                                           [NotNullWhen(true)]  out ChargingPeriod?      ChargingPeriod,
                                           [NotNullWhen(false)] out String?              ErrorResponse)

            => TryParseCBOR(CBOR,
                            out ChargingPeriod,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a charging period.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="ChargingPeriod">The charging period.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomChargingPeriodParser">An optional delegate to read custom charging periods.</param>
        public static Boolean TryParseCBOR(CBORValue                                     CBOR,
                                           [NotNullWhen(true)]  out ChargingPeriod?      ChargingPeriod,
                                           [NotNullWhen(false)] out String?              ErrorResponse,
                                           CustomCBORParserDelegate<ChargingPeriod>?     CustomChargingPeriodParser)
        {

            try
            {

                ChargingPeriod = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a charging period is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("startPeriod",
                                              "start of the charging period",
                                              OCPPCBORExtensions.TryParseTimestamp,
                                              out DateTimeOffset StartPeriod,
                                              out ErrorResponse))
                {
                    return false;
                }

                Tariff_Id? TariffId = null;

                if (CBOR.ParseOptionalText("tariffId",
                                           "tariff identification",
                                           out var tariffIdText,
                                           out ErrorResponse))
                {

                    if (!Tariff_Id.TryParse(tariffIdText!, out var tariffId))
                    {
                        ErrorResponse = $"Invalid tariff identification '{tariffIdText}'!";
                        return false;
                    }

                    TariffId = tariffId;

                }

                if (ErrorResponse is not null)
                    return false;

                CBOR.ParseOptionalList<CostDimension>("dimensions",
                                                      "cost dimensions",
                                                      CostDimension.TryParseCBOR,
                                                      out var Dimensions,
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


                ChargingPeriod = new ChargingPeriod(
                                     StartPeriod,
                                     Dimensions,
                                     TariffId,
                                     CustomData
                                 );

                if (CustomChargingPeriodParser is not null)
                    ChargingPeriod = CustomChargingPeriodParser(CBOR,
                                                                ChargingPeriod);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ChargingPeriod  = null;
                ErrorResponse   = "The given CBOR representation of a charging period is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<ChargingPeriod>.TryParse(CBOR, out ChargingPeriod, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a charging period - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<ChargingPeriod>.TryParse(CBORValue                         CBOR,
                                                                  out ChargingPeriod                Value,
                                                                  [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomChargingPeriodSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this charging period: the keys of its
        /// JSON object, its start a timestamp.
        /// </summary>
        /// <param name="CustomChargingPeriodSerializer">A delegate to serialize custom charging periods.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<ChargingPeriod>? CustomChargingPeriodSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("dimensions",   OCPPCBORExtensions.Array(Dimensions, dimension => dimension.ToCBOR())),
                           ("tariffId",     OCPPCBORExtensions.Text(TariffId?.ToString())),
                           ("startPeriod",  StartPeriod.ToCBOR()),
                           ("customData",   CustomData?.ToCBOR())
                       );

            return CustomChargingPeriodSerializer is not null
                       ? CustomChargingPeriodSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region IEquatable<ChargingPeriod> Members

        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="Object">A charging period to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is ChargingPeriod chargingPeriod &&
                   Equals(chargingPeriod);


        /// <summary>
        /// Compares two charging periods for equality.
        /// </summary>
        /// <param name="ChargingPeriod">A charging period to compare with.</param>
        public Boolean Equals(ChargingPeriod? ChargingPeriod)

            => ChargingPeriod is not null &&

               StartPeriod.Equals(ChargingPeriod.StartPeriod) &&
               Nullable.Equals(TariffId, ChargingPeriod.TariffId) &&
               Dimensions.SequenceEqual(ChargingPeriod.Dimensions) &&

             ((CustomData is null     && ChargingPeriod.CustomData is null) ||
              (CustomData is not null && CustomData.Equals(ChargingPeriod.CustomData)));

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

            => $"From {StartPeriod.ToISO8601()}{(TariffId.HasValue ? $" ({TariffId})" : "")}: {Dimensions.AggregateWith(", ")}";

        #endregion

    }

}
