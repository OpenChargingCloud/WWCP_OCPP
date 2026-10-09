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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A price.
    /// </summary>
    public readonly struct Price : IEquatable<Price>,
                                   IComparable<Price>,
                                   IComparable
    {

        #region Properties

        /// <summary>
        /// Price/Cost excluding taxes, or null where only the price including
        /// taxes is known.
        /// </summary>
        /// <remarks>
        /// Each of the two may be absent in OCPP 2.1, as long as the other is
        /// there; both were required here, and a price with only one of them
        /// was refused.
        /// </remarks>
        [Optional]
        public Decimal?              ExcludingTaxes    { get; }

        /// <summary>
        /// Price/Cost including taxes, or null where only the price excluding
        /// taxes is known.
        /// </summary>
        [Optional]
        public Decimal?              IncludingTaxes    { get; }

        /// <summary>
        /// The optional tax percentages that were used to calculate inclTax from exclTax(for displaying/printing on invoices).
        /// May be absent for a total cost field that contains cost from multiple components with different tax rates.
        /// </summary>
        [Optional]
        public IEnumerable<TaxRate>  TaxRates          { get; }

        /// <summary>
        /// An optional custom data object allowing to store any kind of
        /// customer specific data.
        /// </summary>
        [Optional]
        public CustomData?           CustomData        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price.
        /// </summary>
        /// <param name="ExcludingTaxes">Price/Cost excluding taxes.</param>
        /// <param name="IncludingTaxes">Price/Cost including taxes.</param>
        /// <param name="TaxRates">The optional tax percentages that were used to calculate inclTax from exclTax.</param>
        /// <param name="CustomData">An optional custom data object allowing to store any kind of customer specific data.</param>
        public Price(Decimal?               ExcludingTaxes,
                     Decimal?               IncludingTaxes,
                     IEnumerable<TaxRate>?  TaxRates     = null,
                     CustomData?            CustomData   = null)
        {

            this.ExcludingTaxes  = ExcludingTaxes;
            this.IncludingTaxes  = IncludingTaxes;
            this.TaxRates        = TaxRates?.Distinct() ?? [];
            this.CustomData      = CustomData;

            unchecked
            {

                hashCode = (this.ExcludingTaxes?.GetHashCode() ?? 0) * 7 ^
                           (this.IncludingTaxes?.GetHashCode() ?? 0) * 5 ^
                            this.TaxRates.       CalcHashCode()      * 3 ^
                           (this.CustomData?.    GetHashCode() ?? 0);

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description": "Price with and without tax. At least one of _exclTax_, _inclTax_ must be present.",
        //     "javaType": "Price",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "exclTax": {
        //             "description": "Price/cost excluding tax. Can be absent if _inclTax_ is present.",
        //             "type": "number"
        //         },
        //         "inclTax": {
        //             "description": "Price/cost including tax. Can be absent if _exclTax_ is present.",
        //             "type": "number"
        //         },
        //         "taxRates": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/TaxRateType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 5
        //         },
        //         "customData": {
        //             "$ref": "#/definitions/CustomDataType"
        //         }
        //     }
        // }

        #endregion

        #region (static) Parse   (JSON, CustomPriceParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPriceParser">An optional delegate to parse custom price JSON objects.</param>
        public static Price Parse(JObject                                JSON,
                                  CustomJObjectParserDelegate<Price>?    CustomPriceParser     = null,
                                  CustomJObjectParserDelegate<TaxRate>?  CustomTaxRateParser   = null)
        {

            if (TryParse(JSON,
                         out var price,
                         out var errorResponse,
                         CustomPriceParser,
                         CustomTaxRateParser))
            {
                return price;
            }

            throw new ArgumentException("The given JSON representation of a price is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out Price, out ErrorResponse, CustomPriceParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a price.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Price">The parsed price.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                           JSON,
                                       [NotNullWhen(true)]  out Price    Price,
                                       [NotNullWhen(false)] out String?  ErrorResponse)

            => TryParse(JSON,
                        out Price,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a price.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="Price">The parsed price.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomPriceParser">An optional delegate to parse custom price JSON objects.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out Price         Price,
                                       [NotNullWhen(false)] out String?       ErrorResponse,
                                       CustomJObjectParserDelegate<Price>?    CustomPriceParser     = null,
                                       CustomJObjectParserDelegate<TaxRate>?  CustomTaxRateParser   = null)
        {

            try
            {

                Price = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ExcludingTaxes    [optional]

                if (JSON.ParseOptional("exclTax",
                                       "price excluding taxes",
                                       out Decimal? ExcludingTaxes,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse IncludingTaxes    [optional]

                if (JSON.ParseOptional("inclTax",
                                       "price including taxes",
                                       out Decimal? IncludingTaxes,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                if (!ExcludingTaxes.HasValue && !IncludingTaxes.HasValue)
                {
                    ErrorResponse = "At least one of 'exclTax' and 'inclTax' must be present!";
                    return false;
                }

                #region Parse TaxRates          [optional]

                // "taxRates", as the schema has it - it was "taxRate".
                if (JSON.ParseOptionalHashSet("taxRates",
                                              "tax rates",
                                              TaxRate.TryParse,
                                              out HashSet<TaxRate> TaxRates,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion

                #region Parse CustomData        [optional]

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


                Price = new Price(
                            ExcludingTaxes,
                            IncludingTaxes,
                            TaxRates,
                            CustomData
                        );


                if (CustomPriceParser is not null)
                    Price = CustomPriceParser(JSON,
                                              Price);

                return true;

            }
            catch (Exception e)
            {
                Price          = default;
                ErrorResponse  = "The given JSON representation of a price is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomPriceSerializer = null,CustomTaxRateSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom TaxRate JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Price>?    CustomPriceSerializer     = null,
                              CustomJObjectSerializerDelegate<TaxRate>?  CustomTaxRateSerializer   = null)
        {

            var json = JSONObject.Create(

                           ExcludingTaxes.HasValue
                               ? new JProperty("exclTax",      ExcludingTaxes.Value)
                               : null,

                           IncludingTaxes.HasValue
                               ? new JProperty("inclTax",      IncludingTaxes.Value)
                               : null,

                           TaxRates.Any()
                               ? new JProperty("taxRates",     new JArray(TaxRates.Select(taxRate => taxRate.ToJSON(CustomTaxRateSerializer))))
                               : null,

                           CustomData is not null
                               ? new JProperty("customData",   CustomData.ToJSON())
                               : null

                       );

            return CustomPriceSerializer is not null
                       ? CustomPriceSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this price.
        /// </summary>
        public Price Clone()

            => new (
                   ExcludingTaxes,
                   IncludingTaxes,
                   TaxRates.Select(taxRate => taxRate.Clone()),
                   CustomData
               );

        #endregion


        #region Static definitions

        /// <summary>
        /// Zero
        /// </summary>
        public static Price Zero { get; }
            = new (0, 0);

        #endregion


        #region Operator overloading

        #region Operator == (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Price Price1,
                                           Price Price2)

            => Price1.Equals(Price2);

        #endregion

        #region Operator != (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Price Price1,
                                           Price Price2)

            => !Price1.Equals(Price2);

        #endregion

        #region Operator <  (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Price Price1,
                                          Price Price2)

            => Price1.CompareTo(Price2) < 0;

        #endregion

        #region Operator <= (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Price Price1,
                                           Price Price2)

            => Price1.CompareTo(Price2) <= 0;

        #endregion

        #region Operator >  (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Price Price1,
                                          Price Price2)

            => Price1.CompareTo(Price2) > 0;

        #endregion

        #region Operator >= (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Price Price1,
                                           Price Price2)

            => Price1.CompareTo(Price2) >= 0;

        #endregion


        #region Operator +  (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Price operator + (Price Price1,
                                        Price Price2)

            => new (
                   Price1.ExcludingTaxes + Price2.ExcludingTaxes,
                   Price1.IncludingTaxes + Price2.IncludingTaxes,
                   Price1.TaxRates.Concat(Price2.TaxRates)
               );

        #endregion

        #region Operator -  (Price1, Price2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Price1">A price.</param>
        /// <param name="Price2">Another price.</param>
        /// <returns>true|false</returns>
        public static Price operator - (Price Price1,
                                        Price Price2)

            => new (
                   Price1.ExcludingTaxes - Price2.ExcludingTaxes,
                   Price1.IncludingTaxes - Price2.IncludingTaxes,
                   Price1.TaxRates.Concat(Price2.TaxRates)
               );

        #endregion

        #endregion

        #region IComparable<Price> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two prices.
        /// </summary>
        /// <param name="Object">A price to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Price price
                   ? CompareTo(price)
                   : throw new ArgumentException("The given object is not a price!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(Price)

        /// <summary>
        /// Compares two prices.
        /// </summary>
        /// <param name="Price">A price to compare with.</param>
        public Int32 CompareTo(Price Price)
        {

            var c = Nullable.Compare(ExcludingTaxes, Price.ExcludingTaxes);

            if (c == 0)
                c = Nullable.Compare(IncludingTaxes, Price.IncludingTaxes);

            if (c == 0)
                c = TaxRates.Count().CompareTo(Price.TaxRates.Count());

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<Price> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two prices for equality.
        /// </summary>
        /// <param name="Object">A price to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is Price price &&
                   Equals(price);

        #endregion

        #region Equals(Price)

        /// <summary>
        /// Compares two prices for equality.
        /// </summary>
        /// <param name="Price">A price to compare with.</param>
        public Boolean Equals(Price Price)

            => Nullable.Equals(ExcludingTaxes, Price.ExcludingTaxes) &&
               Nullable.Equals(IncludingTaxes, Price.IncludingTaxes) &&

               TaxRates.Count().Equals(Price.TaxRates.Count()) &&
               TaxRates.All(taxRate => Price.TaxRates.Contains(taxRate)) &&

             ((CustomData is null && Price.CustomData is null) ||
              (CustomData is not null && CustomData.Equals(Price.CustomData)));

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

            => $"{ExcludingTaxes?.ToString() ?? "-"} excl. taxes, {IncludingTaxes?.ToString() ?? "-"} incl. taxes, {TaxRates.Count()} tax rates";

        #endregion

    }

}
