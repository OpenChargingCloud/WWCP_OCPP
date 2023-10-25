/*
 * Copyright (c) 2014-2023 GraphDefined GmbH
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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// The price of a charging session.
    /// </summary>
    public readonly struct Price : IEquatable<Price>,
                                   IComparable<Price>,
                                   IComparable
    {

        #region Properties

        /// <summary>
        /// Price/Cost excluding VAT.
        /// </summary>
        [Mandatory]
        public Decimal   ExcludingVAT    { get; }

        /// <summary>
        /// Price/Cost including VAT.
        /// </summary>
        [Optional]
        public Decimal?  IncludingVAT    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new price for a charging session.
        /// </summary>
        /// <param name="ExcludingVAT">Price/Cost excluding VAT.</param>
        /// <param name="IncludingVAT">Price/Cost including VAT.</param>
        public Price(Decimal   ExcludingVAT,
                     Decimal?  IncludingVAT   = null)
        {

            this.ExcludingVAT  = ExcludingVAT;
            this.IncludingVAT  = IncludingVAT;

        }

        #endregion


        #region (static) Parse   (JSON, CustomPriceParser = null)

        /// <summary>
        /// Parse the given JSON representation of a price.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPriceParser">A delegate to parse custom price JSON objects.</param>
        public static Price Parse(JObject                              JSON,
                                  CustomJObjectParserDelegate<Price>?  CustomPriceParser   = null)
        {

            if (TryParse(JSON,
                         out var price,
                         out var errorResponse,
                         CustomPriceParser))
            {
                return price;
            }

            throw new ArgumentException("The given JSON representation of a price is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, CustomPriceParser = null)

        /// <summary>
        /// Try to parse the given JSON representation of a price.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomPriceParser">A delegate to parse custom price JSON objects.</param>
        public static Price? TryParse(JObject                              JSON,
                                      CustomJObjectParserDelegate<Price>?  CustomPriceParser   = null)
        {

            if (TryParse(JSON,
                         out var price,
                         out var errorResponse,
                         CustomPriceParser))
            {
                return price;
            }

            return default;

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
        public static Boolean TryParse(JObject      JSON,
                                       out Price    Price,
                                       out String?  ErrorResponse)

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
        /// <param name="CustomPriceParser">A delegate to parse custom price JSON objects.</param>
        public static Boolean TryParse(JObject                              JSON,
                                       out Price                            Price,
                                       out String?                          ErrorResponse,
                                       CustomJObjectParserDelegate<Price>?  CustomPriceParser   = null)
        {

            try
            {

                Price = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ExcludingVAT    [mandatory]

                if (!JSON.ParseMandatory("excl_vat",
                                         "price excluding VAT",
                                         out Decimal ExcludingVAT,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse IncludingVAT    [optional]

                if (JSON.ParseOptional("incl_vat",
                                       "price including VAT",
                                       out Decimal? IncludingVAT,
                                       out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                #endregion


                Price = new Price(
                            ExcludingVAT,
                            IncludingVAT
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

        #region ToJSON(CustomPriceSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomPriceSerializer">A delegate to serialize custom price JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<Price>? CustomPriceSerializer = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("excl_vat",  ExcludingVAT),

                           IncludingVAT.HasValue
                               ? new JProperty("incl_vat",  IncludingVAT)
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

            => new (ExcludingVAT,
                    IncludingVAT);

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
                   Price1.ExcludingVAT       +  Price2.ExcludingVAT,
                  (Price1.IncludingVAT ?? 0) + (Price2.IncludingVAT ?? 0)
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
                   Price1.ExcludingVAT       -  Price2.ExcludingVAT,
                  (Price1.IncludingVAT ?? 0) - (Price2.IncludingVAT ?? 0)
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

            var c = ExcludingVAT.CompareTo(Price.ExcludingVAT);

            if (c == 0 && IncludingVAT.HasValue && Price.IncludingVAT.HasValue)
                c = IncludingVAT.Value.CompareTo(Price.IncludingVAT.Value);

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

            => ExcludingVAT.Equals(Price.ExcludingVAT) &&

            ((!IncludingVAT.HasValue && !Price.IncludingVAT.HasValue) ||
              (IncludingVAT.HasValue &&  Price.IncludingVAT.HasValue && IncludingVAT.Value.Equals(Price.IncludingVAT.Value)));

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return ExcludingVAT. GetHashCode() * 3 ^
                       IncludingVAT?.GetHashCode() ?? 0;

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(

                   ExcludingVAT,

                   IncludingVAT.HasValue
                       ? " / " + IncludingVAT
                       : ""

               );

        #endregion

    }

}
