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
    /// The cost of a transaction, running or final: its total, and the sums of
    /// its fixed, energy, charging time, idle time and reservation cost.
    /// </summary>
    public class TotalCost : IEquatable<TotalCost>,
                             ICBORSerializable<TotalCost>
    {

        #region Properties

        /// <summary>
        /// The currency of the cost.
        /// </summary>
        [Mandatory]
        public Currency     Currency            { get; }

        /// <summary>
        /// The type of the cost: normal, or the minimum or the maximum cost.
        /// </summary>
        [Mandatory]
        public TariffCosts  TypeOfCost          { get; }

        /// <summary>
        /// The total of the fixed, energy, charging time, idle time and reservation cost.
        /// Its tax rates are never written - a total has none in OCPP 2.1.
        /// </summary>
        [Mandatory]
        public Price        Total               { get; }

        /// <summary>
        /// The sum of all flat fees, but those of a reservation.
        /// </summary>
        [Optional]
        public Price?       Fixed               { get; }

        /// <summary>
        /// The cost of the energy.
        /// </summary>
        [Optional]
        public Price?       Energy              { get; }

        /// <summary>
        /// The cost of the duration of charging.
        /// </summary>
        [Optional]
        public Price?       ChargingTime        { get; }

        /// <summary>
        /// The cost of the idle time, including its fixed price components.
        /// </summary>
        [Optional]
        public Price?       IdleTime            { get; }

        /// <summary>
        /// The time-based cost of the reservation.
        /// </summary>
        [Optional]
        public Price?       ReservationTime     { get; }

        /// <summary>
        /// The flat fees of the reservation.
        /// </summary>
        [Optional]
        public Price?       ReservationFixed    { get; }

        /// <summary>
        /// An optional custom data object allowing to store any kind of customer specific data.
        /// </summary>
        [Optional]
        public CustomData?  CustomData          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new cost of a transaction.
        /// </summary>
        /// <param name="Currency">The currency of the cost.</param>
        /// <param name="TypeOfCost">The type of the cost: normal, or the minimum or the maximum cost.</param>
        /// <param name="Total">The total of the fixed, energy, charging time, idle time and reservation cost.</param>
        /// <param name="Fixed">The sum of all flat fees, but those of a reservation.</param>
        /// <param name="Energy">The cost of the energy.</param>
        /// <param name="ChargingTime">The cost of the duration of charging.</param>
        /// <param name="IdleTime">The cost of the idle time, including its fixed price components.</param>
        /// <param name="ReservationTime">The time-based cost of the reservation.</param>
        /// <param name="ReservationFixed">The flat fees of the reservation.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public TotalCost(Currency     Currency,
                         TariffCosts  TypeOfCost,
                         Price        Total,
                         Price?       Fixed              = null,
                         Price?       Energy             = null,
                         Price?       ChargingTime       = null,
                         Price?       IdleTime           = null,
                         Price?       ReservationTime    = null,
                         Price?       ReservationFixed   = null,
                         CustomData?  CustomData         = null)
        {

            this.Currency          = Currency;
            this.TypeOfCost        = TypeOfCost;
            this.Total             = Total;
            this.Fixed             = Fixed;
            this.Energy            = Energy;
            this.ChargingTime      = ChargingTime;
            this.IdleTime          = IdleTime;
            this.ReservationTime   = ReservationTime;
            this.ReservationFixed  = ReservationFixed;
            this.CustomData        = CustomData;

            unchecked
            {
                hashCode = this.Currency.         GetHashCode()       * 29 ^
                           this.TypeOfCost.       GetHashCode()       * 23 ^
                           this.Total.            GetHashCode()       * 19 ^
                          (this.Fixed?.           GetHashCode() ?? 0) * 17 ^
                          (this.Energy?.          GetHashCode() ?? 0) * 13 ^
                          (this.ChargingTime?.    GetHashCode() ?? 0) * 11 ^
                          (this.IdleTime?.        GetHashCode() ?? 0) *  7 ^
                          (this.ReservationTime?. GetHashCode() ?? 0) *  5 ^
                          (this.ReservationFixed?.GetHashCode() ?? 0) *  3 ^
                          (this.CustomData?.      GetHashCode() ?? 0);
            }

        }

        #endregion


        #region Documentation

        // "TotalCostType": {
        //   "description": "This contains the cost calculated during a transaction. It is used both for running cost and final cost of the transaction.",
        //   "javaType": "TotalCost",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "currency":         { "description": "Currency of the costs in ISO 4217 Code.", "type": "string", "maxLength": 3 },
        //     "typeOfCost":       { "$ref": "#/definitions/TariffCostEnumType" },
        //     "fixed":            { "$ref": "#/definitions/PriceType" },
        //     "energy":           { "$ref": "#/definitions/PriceType" },
        //     "chargingTime":     { "$ref": "#/definitions/PriceType" },
        //     "idleTime":         { "$ref": "#/definitions/PriceType" },
        //     "reservationTime":  { "$ref": "#/definitions/PriceType" },
        //     "reservationFixed": { "$ref": "#/definitions/PriceType" },
        //     "total":            { "$ref": "#/definitions/TotalPriceType" },
        //     "customData":       { "$ref": "#/definitions/CustomDataType" }
        //   },
        //   "required": [ "currency", "typeOfCost", "total" ]
        // }
        //
        // "TotalPriceType": {
        //   "description": "Total cost with and without tax. Contains the total of energy, charging time, idle time, fixed and reservation costs including and/or excluding tax.",
        //   "javaType": "TotalPrice",
        //   "type": "object",
        //   "additionalProperties": false,
        //   "properties": {
        //     "exclTax":    { "description": "Price/cost excluding tax. Can be absent if _inclTax_ is present.", "type": "number" },
        //     "inclTax":    { "description": "Price/cost including tax. Can be absent if _exclTax_ is present.", "type": "number" },
        //     "customData": { "$ref": "#/definitions/CustomDataType" }
        //   }
        // }

        #endregion

        #region (static) TryParse    (JSON, out TotalCost, out ErrorResponse, CustomTotalCostParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a cost of a transaction.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TotalCost">The parsed cost.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out TotalCost?       TotalCost,
                                       [NotNullWhen(false)] out String?          ErrorResponse)

            => TryParse(JSON,
                        out TotalCost,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a cost of a transaction.
        /// </summary>
        /// <param name="JSON">The JSON to be parsed.</param>
        /// <param name="TotalCost">The parsed cost.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTotalCostParser">An optional delegate to parse custom costs.</param>
        public static Boolean TryParse(JObject                                   JSON,
                                       [NotNullWhen(true)]  out TotalCost?       TotalCost,
                                       [NotNullWhen(false)] out String?          ErrorResponse,
                                       CustomJObjectParserDelegate<TotalCost>?   CustomTotalCostParser)
        {

            try
            {

                TotalCost = null;

                if (!JSON.ParseMandatory("currency",
                                         "currency",
                                         org.GraphDefined.Vanaheimr.Illias.Currency.TryParse,
                                         out Currency Currency,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (!JSON.ParseMandatory("typeOfCost",
                                         "type of cost",
                                         TariffCostsExtensions.TryParse,
                                         out TariffCosts TypeOfCost,
                                         out ErrorResponse))
                {
                    return false;
                }

                if (!JSON.ParseMandatoryJSON("total",
                                             "total cost",
                                             Price.TryParse,
                                             out Price Total,
                                             out ErrorResponse))
                {
                    return false;
                }

                var prices = new Dictionary<String, Price?>();

                foreach (var key in new[] { "fixed", "energy", "chargingTime", "idleTime", "reservationTime", "reservationFixed" })
                {

                    if (JSON.ParseOptionalJSON(key,
                                               key,
                                               Price.TryParse,
                                               out Price? price,
                                               out ErrorResponse))
                    {
                        if (ErrorResponse is not null)
                            return false;
                    }

                    prices[key] = price;

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


                TotalCost = new TotalCost(
                                Currency,
                                TypeOfCost,
                                Total,
                                prices["fixed"],
                                prices["energy"],
                                prices["chargingTime"],
                                prices["idleTime"],
                                prices["reservationTime"],
                                prices["reservationFixed"],
                                CustomData
                            );

                if (CustomTotalCostParser is not null)
                    TotalCost = CustomTotalCostParser(JSON,
                                                      TotalCost);

                return true;

            }
            catch (Exception e)
            {
                TotalCost      = null;
                ErrorResponse  = "The given JSON representation of a cost of a transaction is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTotalCostSerializer = null, CustomPriceSerializer = null, CustomTaxRateSerializer = null)

        /// <summary>
        /// Return a JSON representation of this cost.
        /// </summary>
        /// <param name="CustomTotalCostSerializer">A delegate to serialize custom costs.</param>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom prices.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom tax rates.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TotalCost>?  CustomTotalCostSerializer   = null,
                              CustomJObjectSerializerDelegate<Price>?      CustomPriceSerializer       = null,
                              CustomJObjectSerializerDelegate<TaxRate>?    CustomTaxRateSerializer     = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("currency",           Currency.ISOCode),
                                 new JProperty("typeOfCost",         TypeOfCost.AsText()),

                           Fixed.HasValue
                               ? new JProperty("fixed",              Fixed.           Value.ToJSON(CustomPriceSerializer, CustomTaxRateSerializer))
                               : null,

                           Energy.HasValue
                               ? new JProperty("energy",             Energy.          Value.ToJSON(CustomPriceSerializer, CustomTaxRateSerializer))
                               : null,

                           ChargingTime.HasValue
                               ? new JProperty("chargingTime",       ChargingTime.    Value.ToJSON(CustomPriceSerializer, CustomTaxRateSerializer))
                               : null,

                           IdleTime.HasValue
                               ? new JProperty("idleTime",           IdleTime.        Value.ToJSON(CustomPriceSerializer, CustomTaxRateSerializer))
                               : null,

                           ReservationTime.HasValue
                               ? new JProperty("reservationTime",    ReservationTime. Value.ToJSON(CustomPriceSerializer, CustomTaxRateSerializer))
                               : null,

                           ReservationFixed.HasValue
                               ? new JProperty("reservationFixed",   ReservationFixed.Value.ToJSON(CustomPriceSerializer, CustomTaxRateSerializer))
                               : null,

                                 // A total has no tax rates.
                                 new JProperty("total",              new Price(Total.ExcludingTaxes,
                                                                               Total.IncludingTaxes,
                                                                               null,
                                                                               Total.CustomData).ToJSON(CustomPriceSerializer)),

                           CustomData is not null
                               ? new JProperty("customData",         CustomData.ToJSON())
                               : null

                       );

            return CustomTotalCostSerializer is not null
                       ? CustomTotalCostSerializer(this, json)
                       : json;

        }

        #endregion

        #region (static) TryParseCBOR(CBOR, out TotalCost, out ErrorResponse, CustomTotalCostParser = null)

        /// <summary>
        /// Try to read the given CBOR representation of a cost of a transaction.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="TotalCost">The cost.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParseCBOR(CBORValue                                CBOR,
                                           [NotNullWhen(true)]  out TotalCost?      TotalCost,
                                           [NotNullWhen(false)] out String?         ErrorResponse)

            => TryParseCBOR(CBOR,
                            out TotalCost,
                            out ErrorResponse,
                            null);


        /// <summary>
        /// Try to read the given CBOR representation of a cost of a transaction.
        /// </summary>
        /// <param name="CBOR">The CBOR to be read.</param>
        /// <param name="TotalCost">The cost.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTotalCostParser">An optional delegate to read custom costs.</param>
        public static Boolean TryParseCBOR(CBORValue                                CBOR,
                                           [NotNullWhen(true)]  out TotalCost?      TotalCost,
                                           [NotNullWhen(false)] out String?         ErrorResponse,
                                           CustomCBORParserDelegate<TotalCost>?     CustomTotalCostParser)
        {

            try
            {

                TotalCost = null;

                if (CBOR.Kind != CBORValueKind.Map)
                {
                    ErrorResponse = "The given CBOR representation of a cost of a transaction is not a map!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("currency",
                                             "currency",
                                             out var currencyText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!org.GraphDefined.Vanaheimr.Illias.Currency.TryParse(currencyText, out var Currency))
                {
                    ErrorResponse = $"Invalid currency '{currencyText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryText("typeOfCost",
                                             "type of cost",
                                             out var typeOfCostText,
                                             out ErrorResponse))
                {
                    return false;
                }

                if (!TariffCostsExtensions.TryParse(typeOfCostText, out var TypeOfCost))
                {
                    ErrorResponse = $"Invalid type of cost '{typeOfCostText}'!";
                    return false;
                }

                if (!CBOR.ParseMandatoryValue("total",
                                              "total cost",
                                              OCPPv2_1.Price.TryParseCBOR,
                                              out Price Total,
                                              out ErrorResponse))
                {
                    return false;
                }

                var prices = new Dictionary<String, Price?>();

                foreach (var key in new[] { "fixed", "energy", "chargingTime", "idleTime", "reservationTime", "reservationFixed" })
                {

                    CBOR.ParseOptionalValue(key,
                                            key,
                                            OCPPv2_1.Price.TryParseCBOR,
                                            out Price? price,
                                            out ErrorResponse);

                    if (ErrorResponse is not null)
                        return false;

                    prices[key] = price;

                }

                CBOR.ParseOptional("customData",
                                   "custom data",
                                   OCPPCBORExtensions.TryParseCustomData,
                                   out CustomData? CustomData,
                                   out ErrorResponse);

                if (ErrorResponse is not null)
                    return false;


                TotalCost = new TotalCost(
                                Currency,
                                TypeOfCost,
                                Total,
                                prices["fixed"],
                                prices["energy"],
                                prices["chargingTime"],
                                prices["idleTime"],
                                prices["reservationTime"],
                                prices["reservationFixed"],
                                CustomData
                            );

                if (CustomTotalCostParser is not null)
                    TotalCost = CustomTotalCostParser(CBOR,
                                                      TotalCost);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                TotalCost      = null;
                ErrorResponse  = "The given CBOR representation of a cost of a transaction is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) ICBORSerializable<TotalCost>.TryParse(CBOR, out TotalCost, out ErrorResponse)

        /// <summary>
        /// Try to read the given CBOR representation of a cost of a transaction - see TryParseCBOR(),
        /// which is not called TryParse, so that a method group of TryParse stays the JSON one.
        /// </summary>
        static Boolean ICBORSerializable<TotalCost>.TryParse(CBORValue                         CBOR,
                                                             out TotalCost                     Value,
                                                             [NotNullWhen(false)] out String?  ErrorResponse)
        {
            var result = TryParseCBOR(CBOR, out var value, out ErrorResponse);
            Value = value!;
            return result;
        }

        #endregion

        #region ToCBOR(CustomTotalCostSerializer = null)

        /// <summary>
        /// Return the CBOR representation of this cost: the keys of its JSON object.
        /// </summary>
        /// <param name="CustomTotalCostSerializer">A delegate to serialize custom costs.</param>
        public CBORValue ToCBOR(CustomCBORSerializerDelegate<TotalCost>? CustomTotalCostSerializer = null)
        {

            var cbor = OCPPCBORExtensions.Map(
                           ("currency",          CBORValue.FromText(Currency.ISOCode)),
                           ("typeOfCost",        CBORValue.FromText(TypeOfCost.AsText())),
                           ("fixed",             Fixed?.           ToCBOR()),
                           ("energy",            Energy?.          ToCBOR()),
                           ("chargingTime",      ChargingTime?.    ToCBOR()),
                           ("idleTime",          IdleTime?.        ToCBOR()),
                           ("reservationTime",   ReservationTime?. ToCBOR()),
                           ("reservationFixed",  ReservationFixed?.ToCBOR()),
                           // A total has no tax rates.
                           ("total",             new Price(Total.ExcludingTaxes,
                                                           Total.IncludingTaxes,
                                                           null,
                                                           Total.CustomData).ToCBOR()),
                           ("customData",        CustomData?.ToCBOR())
                       );

            return CustomTotalCostSerializer is not null
                       ? CustomTotalCostSerializer(this, cbor)
                       : cbor;

        }

        #endregion


        #region IEquatable<TotalCost> Members

        /// <summary>
        /// Compares two costs for equality.
        /// </summary>
        /// <param name="Object">A cost to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TotalCost totalCost &&
                   Equals(totalCost);


        /// <summary>
        /// Compares two costs for equality.
        /// </summary>
        /// <param name="TotalCost">A cost to compare with.</param>
        public Boolean Equals(TotalCost? TotalCost)

            => TotalCost is not null &&

               Currency.  Equals(TotalCost.Currency)   &&
               TypeOfCost.Equals(TotalCost.TypeOfCost) &&
               Total.     Equals(TotalCost.Total)      &&

               Nullable.Equals(Fixed,            TotalCost.Fixed)            &&
               Nullable.Equals(Energy,           TotalCost.Energy)           &&
               Nullable.Equals(ChargingTime,     TotalCost.ChargingTime)     &&
               Nullable.Equals(IdleTime,         TotalCost.IdleTime)         &&
               Nullable.Equals(ReservationTime,  TotalCost.ReservationTime)  &&
               Nullable.Equals(ReservationFixed, TotalCost.ReservationFixed) &&

             ((CustomData is null     && TotalCost.CustomData is null) ||
              (CustomData is not null && CustomData.Equals(TotalCost.CustomData)));

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

            => $"{TypeOfCost.AsText()}: {Total} {Currency.ISOCode}";

        #endregion

    }

}
