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

#endregion

namespace cloud.charging.open.protocols.OCPPv2_1
{

    /// <summary>
    /// A TariffFixed tariff element.
    /// </summary>
    public class TariffFixed : IEquatable<TariffFixed>
    {

        #region Properties

        /// <summary>
        /// The enumeration of tariff prices and conditions.
        /// </summary>
        [Mandatory]
        public IEnumerable<TariffFixedPrice>  Prices      { get;  }

        /// <summary>
        /// The optional enumeration of applicable tax percentages for this tariff dimension.
        /// If omitted, no tax is applicable.
        /// Not providing a tax is different from 0% tax, which would be a value of 0.0 here.
        /// </summary>
        [Optional]
        public IEnumerable<TaxRate>           TaxRates    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new TariffFixed tariff element.
        /// </summary>
        /// <param name="Prices">An enumeration of tariff prices and conditions.</param>
        /// <param name="TaxRates">An optional enumeration of applicable tax percentages for this tariff dimension.</param>
        public TariffFixed(IEnumerable<TariffFixedPrice>  Prices,
                           IEnumerable<TaxRate>?          TaxRates   = null)
        {

            if (!Prices.Any())
                throw new ArgumentNullException(nameof(Prices), "The given enumeration of prices must not be empty!");

            this.Prices    = Prices.   Distinct();
            this.TaxRates  = TaxRates?.Distinct() ?? [];

            unchecked
            {

                hashCode = this.Prices.  CalcHashCode() * 3 ^
                           this.TaxRates.CalcHashCode();

            }

        }

        #endregion


        #region Documentation

        // {
        //     "javaType": "TariffFixed",
        //     "type": "object",
        //     "additionalProperties": false,
        //     "properties": {
        //         "prices": {
        //             "type": "array",
        //             "additionalItems": false,
        //             "items": {
        //                 "$ref": "#/definitions/TariffFixedPriceType"
        //             },
        //             "minItems": 1
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
        //     },
        //     "required": [
        //         "prices"
        //     ]
        // }

        #endregion

        #region (static) Parse   (JSON, CustomTariffFixedParser = null)

        /// <summary>
        /// Parse the given JSON representation of a TariffFixed tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomTariffFixedParser">An optional delegate to parse custom TariffFixed tariff element JSON objects.</param>
        public static TariffFixed Parse(JObject                                     JSON,
                                         CustomJObjectParserDelegate<TariffFixed>?  CustomTariffFixedParser   = null)
        {

            if (TryParse(JSON,
                         out var tariffFixed,
                         out var errorResponse,
                         CustomTariffFixedParser))
            {
                return tariffFixed;
            }

            throw new ArgumentException("The given JSON representation of a TariffFixed tariff element is invalid: " + errorResponse,
                                        nameof(JSON));

        }

        #endregion

        #region (static) TryParse(JSON, out TariffFixed, out ErrorResponse, CustomTariffFixedParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffFixed">The parsed tariff element.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject                                JSON,
                                       [NotNullWhen(true)]  out TariffFixed?  TariffFixed,
                                       [NotNullWhen(false)] out String?       ErrorResponse)

            => TryParse(JSON,
                        out TariffFixed,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a tariff element.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="TariffFixed">The parsed tariff element.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomTariffFixedParser">An optional delegate to parse custom tariff element JSON objects.</param>
        public static Boolean TryParse(JObject                                    JSON,
                                       [NotNullWhen(true)]  out TariffFixed?      TariffFixed,
                                       [NotNullWhen(false)] out String?           ErrorResponse,
                                       CustomJObjectParserDelegate<TariffFixed>?  CustomTariffFixedParser   = null)
        {

            try
            {

                TariffFixed = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Prices      [mandatory]

                if (!JSON.ParseMandatoryHashSet("prices",
                                                "fixed prices",
                                                TariffFixedPrice.TryParse,
                                                out HashSet<TariffFixedPrice> Prices,
                                                out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TaxRates    [optional]

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


                TariffFixed = new TariffFixed(
                                  Prices,
                                  TaxRates
                              );


                if (CustomTariffFixedParser is not null)
                    TariffFixed = CustomTariffFixedParser(JSON,
                                                          TariffFixed);

                return true;

            }
            catch (Exception e)
            {
                TariffFixed    = default;
                ErrorResponse  = "The given JSON representation of a TariffFixed tariff element is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomTariffFixedSerializer = null, CustomPriceComponentSerializer = null, ...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomTariffFixedSerializer">A delegate to serialize custom TariffFixed JSON objects.</param>
        /// <param name="CustomTariffFixedPriceSerializer">A delegate to serialize custom TariffFixedPrice JSON objects.</param>
        /// <param name="CustomTariffConditionsSerializer">A delegate to serialize custom TariffConditions JSON objects.</param>
        /// <param name="CustomTaxRateSerializer">A delegate to serialize custom TaxRate JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<TariffFixed>?       CustomTariffFixedSerializer        = null,
                              CustomJObjectSerializerDelegate<TariffFixedPrice>?  CustomTariffFixedPriceSerializer   = null,
                              CustomJObjectSerializerDelegate<TariffConditions>?  CustomTariffConditionsSerializer   = null,
                              CustomJObjectSerializerDelegate<TaxRate>?           CustomTaxRateSerializer            = null)
        {

            var json = JSONObject.Create(

                                 new JProperty("prices",   new JArray(Prices.  Select(tariffFixedPrice => tariffFixedPrice.ToJSON(CustomTariffFixedPriceSerializer,
                                                                                                                                  CustomTariffConditionsSerializer)))),

                           TaxRates.Any()
                               ? new JProperty("taxRate",  new JArray(TaxRates.Select(taxRate          => taxRate.         ToJSON(CustomTaxRateSerializer))))
                               : null

                       );

            return CustomTariffFixedSerializer is not null
                       ? CustomTariffFixedSerializer(this, json)
                       : json;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this TariffFixed tariff element.
        /// </summary>
        public TariffFixed Clone()

            => new (
                   Prices.  Select(tariffFixedPrice => tariffFixedPrice.Clone()),
                   TaxRates.Select(taxRate          => taxRate.         Clone())
               );

        #endregion


        #region Operator overloading

        #region Operator == (TariffFixed1, TariffFixed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffFixed1">A TariffFixed tariff element.</param>
        /// <param name="TariffFixed2">Another TariffFixed tariff element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (TariffFixed? TariffFixed1,
                                           TariffFixed? TariffFixed2)
        {

            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(TariffFixed1, TariffFixed2))
                return true;

            // If one is null, but not both, return false.
            if (TariffFixed1 is null || TariffFixed2 is null)
                return false;

            return TariffFixed1.Equals(TariffFixed2);

        }

        #endregion

        #region Operator != (TariffFixed1, TariffFixed2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="TariffFixed1">A TariffFixed tariff element.</param>
        /// <param name="TariffFixed2">Another TariffFixed tariff element.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (TariffFixed? TariffFixed1,
                                           TariffFixed? TariffFixed2)

            => !(TariffFixed1 == TariffFixed2);

        #endregion

        #endregion

        #region IEquatable<TariffFixed> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two TariffFixed tariff element for equality.
        /// </summary>
        /// <param name="Object">A TariffFixed tariff element to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is TariffFixed tariffFixed &&
                   Equals(tariffFixed);

        #endregion

        #region Equals(TariffFixed)

        /// <summary>
        /// Compares two TariffFixed tariff element for equality.
        /// </summary>
        /// <param name="TariffFixed">A TariffFixed tariff element to compare with.</param>
        public Boolean Equals(TariffFixed? TariffFixed)

            => TariffFixed is not null &&

               Prices.  Count().Equals(TariffFixed.Prices.  Count()) &&
               TaxRates.Count().Equals(TariffFixed.TaxRates.Count()) &&

               Prices.  All(tariffFixedPrice => TariffFixed.Prices.  Contains(tariffFixedPrice)) &&
               TaxRates.All(taxRate          => TariffFixed.TaxRates.Contains(taxRate));

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

            => $"{Prices.Count()} prices, {TaxRates.Count()} tax rates";

        #endregion

    }

}
