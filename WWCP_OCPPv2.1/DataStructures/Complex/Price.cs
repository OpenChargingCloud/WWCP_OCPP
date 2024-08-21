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

using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

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
        /// Price/Cost excluding Taxes.
        /// </summary>
        [Mandatory]
        public Decimal               ExcludingTaxes    { get; }

        /// <summary>
        /// Price/Cost including Taxes.
        /// </summary>
        [Mandatory]
        public Decimal               IncludingTaxes    { get; }

        /// <summary>
        /// The optional tax percentages that were used to calculate inclTax from exclTax(for displaying/printing on invoices).
        /// May be absent for a total cost field that contains cost from multiple components with different tax rates.
        /// </summary>
        [Optional]
        public IEnumerable<TaxRate>  TaxRates          { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price.
        /// </summary>
        /// <param name="ExcludingTaxes">Price/Cost excluding taxes.</param>
        /// <param name="IncludingTaxes">Price/Cost including taxes.</param>
        /// <param name="TaxRates"></param>
        public Price(Decimal                ExcludingTaxes,
                     Decimal                IncludingTaxes,
                     IEnumerable<TaxRate>?  TaxRates   = null)
        {

            this.ExcludingTaxes  = ExcludingTaxes;
            this.IncludingTaxes  = IncludingTaxes;
            this.TaxRates        = TaxRates?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.ExcludingTaxes.GetHashCode() * 5 ^
                           this.IncludingTaxes.GetHashCode() * 3 ^
                           this.TaxRates.      CalcHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "description":             "Price with and without tax\r\n",
        //     "javaType":                "Price",
        //     "type":                    "object",
        //     "additionalProperties":     false,
        //     "properties": {
        //         "exclTax": {
        //             "description":     "Price/cost excluding tax.\r\n",
        //             "type":            "integer",
        //             "minimum":          0.0
        //         },
        //         "inclTax": {
        //             "description":     "Price/cost including tax\r\n",
        //             "type":            "integer",
        //             "minimum":          0.0
        //         },
        //         "taxRate": {
        //             "type":            "array",
        //             "additionalItems":  false,
        //             "items": {
        //                 "$ref":        "#/definitions/TaxRateType"
        //             },
        //             "minItems": 1,
        //             "maxItems": 5
        //         }
        //     },
        //     "required": [
        //         "exclTax",
        //         "inclTax"
        //     ]
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

                #region Parse ExcludingTaxes    [mandatory]

                if (!JSON.ParseMandatory("exclTax",
                                         "price excluding Taxes",
                                         out Decimal ExcludingTaxes,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse IncludingTaxes    [mandatory]

                if (!JSON.ParseMandatory("inclTax",
                                         "price including Taxes",
                                         out Decimal IncludingTaxes,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TaxRates          [optional]

                if (JSON.ParseOptionalHashSet("taxRate",
                                              "tax rates",
                                              TaxRate.TryParse,
                                              out HashSet<TaxRate> TaxRates,
                                              out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Price = new Price(
                            ExcludingTaxes,
                            IncludingTaxes,
                            TaxRates
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

                                 new JProperty("exclTax",  ExcludingTaxes),
                                 new JProperty("inclTax",  IncludingTaxes),

                           TaxRates.Any()
                               ? new JProperty("taxRate",  new JArray(TaxRates.Select(taxRate => taxRate.ToJSON(CustomTaxRateSerializer))))
                               : null

                       );

            return CustomPriceSerializer is not null
                       ? CustomPriceSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this object.
        /// </summary>
        public Price Clone()

            => new (
                   ExcludingTaxes,
                   IncludingTaxes,
                   TaxRates.Select(taxRate => taxRate.Clone())
               );

        #endregion


        public static Price Zero
            = new (0, 0);


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

            var c = ExcludingTaxes.  CompareTo(Price.ExcludingTaxes);

            if (c == 0)
                c = IncludingTaxes.  CompareTo(Price.IncludingTaxes);

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

            => ExcludingTaxes.  Equals(Price.ExcludingTaxes) &&
               IncludingTaxes.  Equals(Price.IncludingTaxes) &&

               TaxRates.Count().Equals(Price.TaxRates.Count()) &&
               TaxRates.All(taxRate => Price.TaxRates.Contains(taxRate));

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

            => $"{ExcludingTaxes} excl. taxes, {IncludingTaxes} incl. taxes, {TaxRates.Count()} tax rates";

        #endregion

    }

}
